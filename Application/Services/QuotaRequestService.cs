using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Koru;
using Koru.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace Application.Services
{
    public class QuotaRequestService : BaseService, IQuotaRequestService
    {
        private readonly IMapper mapper;
        private readonly IQuotaRequestRepository quotaRequestRepository;
        private readonly IAnnouncementRepository announcementRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IKoruRepository koruRepository;

        public QuotaRequestService(IMapper mapper, IUnitOfWork unitOfWork, IQuotaRequestRepository quotaRequestRepository, IAnnouncementRepository announcementRepository, IKoruRepository koruRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.quotaRequestRepository = quotaRequestRepository;
            this.announcementRepository = announcementRepository;
            this.koruRepository = koruRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginationModel<QuotaRequestResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<QuotaRequest> ordersQuery = quotaRequestRepository.GetListWithSubRecords(cancellationToken);
            FilterResponse<QuotaRequest> filterResponse = ordersQuery.ToFilterView(filter);

            var quotaRequests = mapper.Map<List<QuotaRequestResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<QuotaRequestResponseDTO>
            {
                Items = quotaRequests,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<QuotaRequestResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            long userId = httpContextAccessor.HttpContext.GetUserId();
            User user = await unitOfWork.UserRepository.GetByIdAsync(cancellationToken, userId);
            var zone = await koruRepository.GetZone(cancellationToken, userId, user.SelectedRoleId);

            QuotaRequest quotaRequest = await quotaRequestRepository.GetWithSubRecords(cancellationToken, id, zone);

            return new ResponseWrapper<QuotaRequestResponseDTO> { Result = true, Item = mapper.Map<QuotaRequestResponseDTO>(quotaRequest) };
        }

        public async Task<ResponseWrapper<QuotaRequestResponseDTO>> PostAsync(CancellationToken cancellationToken, QuotaRequestDTO quotaRequestDTO)
        {
            QuotaRequest quotaRequest = mapper.Map<QuotaRequest>(quotaRequestDTO);
            quotaRequest.ApplicationStartDate = DateTime.UtcNow;
            var title = $"{quotaRequest.Year} yılı {(quotaRequest.Period == YearPeriodType.Period_1 ? " 1." : " 2.")} dönem {quotaRequest.Type} kontenjan talepleri başlamıştır.";
            var explanation = quotaRequest.Year.ToString() + " yılı" + (quotaRequest.Period == YearPeriodType.Period_1 ? " 1." : " 2.") + $" dönem kontenjan talepleri için bitiş tarihi: {quotaRequest.ApplicationEndDate?.ToString("dd.MM.yyyy")}.";

            await quotaRequestRepository.AddAsync(cancellationToken, quotaRequest);
            await announcementRepository.AddAsync(cancellationToken, new Announcement() { Title = title, Explanation = explanation, PublishDate = DateTime.UtcNow });
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<QuotaRequestResponseDTO> { Result = true, Item = mapper.Map<QuotaRequestResponseDTO>(quotaRequest) };
        }

        

        public async Task<ResponseWrapper<QuotaRequestResponseDTO>> Put(CancellationToken cancellationToken, long id, QuotaRequestDTO quotaRequestDTO)
        {
            QuotaRequest quotaRequest = await quotaRequestRepository.GetByIdAsync(cancellationToken, id);
            var updatedQuotaRequest = mapper.Map(quotaRequestDTO, quotaRequest);

            quotaRequestRepository.Update(updatedQuotaRequest);

            var response = mapper.Map<QuotaRequestResponseDTO>(quotaRequest);

            return new ResponseWrapper<QuotaRequestResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<QuotaRequestResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            QuotaRequest quotaRequest = await quotaRequestRepository.GetByIdAsync(cancellationToken, id);

            quotaRequestRepository.SoftDelete(quotaRequest);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<QuotaRequestResponseDTO> { Result = true };
        }
    }
}
