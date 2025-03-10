using Application.Interfaces;
using Application.Reports.ExcelReports.SubQuotaRequestReports;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class SubQuotaRequestService : BaseService, ISubQuotaRequestService
    {
        private readonly IMapper mapper;
        private readonly ISubQuotaRequestRepository subQuotaRequestRepository;

        public SubQuotaRequestService(IMapper mapper, IUnitOfWork unitOfWork,
            ISubQuotaRequestRepository subQuotaRequestRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.subQuotaRequestRepository = subQuotaRequestRepository;
        }

        public async Task<PaginationModel<SubQuotaRequestPaginateResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<SubQuotaRequestPaginateResponseDTO> ordersQuery = subQuotaRequestRepository.PaginateQuery();
            FilterResponse<SubQuotaRequestPaginateResponseDTO> filterResponse = ordersQuery.ToFilterView(filter);

            var subQuotaRequests = await filterResponse.Query.ToListAsync();

            var response = new PaginationModel<SubQuotaRequestPaginateResponseDTO>
            {
                Items = subQuotaRequests,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetByIdAsync(CancellationToken cancellationToken,
            long id)
        {
            SubQuotaRequest subQuotaRequest = await subQuotaRequestRepository.GetWithSubRecords(cancellationToken, id);

            return new ResponseWrapper<SubQuotaRequestResponseDTO>
            { Result = true, Item = mapper.Map<SubQuotaRequestResponseDTO>(subQuotaRequest) };
        }

        public async Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter)
        {
            filter.pageSize = int.MaxValue;
            filter.page = 1;

            var response = await GetPaginateList(cancellationToken, filter);

            var byteArray = ExportList.ExportListReport(response.Items.OrderBy(x=>x.ExpertiseBranchName).ToList());

            return new ResponseWrapper<byte[]> { Result = true, Item = byteArray };
        }

        public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> GetByProgramId(
            CancellationToken cancellationToken, long programId)
        {
            SubQuotaRequest subQuotaRequest = await subQuotaRequestRepository.GetByProgramIdWithSubRecords(cancellationToken, programId);

            return new ResponseWrapper<SubQuotaRequestResponseDTO>
            { Result = true, Item = mapper.Map<SubQuotaRequestResponseDTO>(subQuotaRequest) };
        }

        public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> PostAsync(CancellationToken cancellationToken,
            SubQuotaRequestDTO subQuotaRequestDTO)
        {
            SubQuotaRequest subQuotaRequest = mapper.Map<SubQuotaRequest>(subQuotaRequestDTO);

            await subQuotaRequestRepository.AddAsync(cancellationToken, subQuotaRequest);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<SubQuotaRequestResponseDTO>
            { Result = true, Item = mapper.Map<SubQuotaRequestResponseDTO>(subQuotaRequest) };
        }

        public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Put(CancellationToken cancellationToken, long id, SubQuotaRequestDTO subQuotaRequestDTO)
        {
            SubQuotaRequest subQuotaRequest = await subQuotaRequestRepository.GetIncluding(cancellationToken,x=>x.Id == id, x=>x.StudentCounts);
            var updatedSubQuotaRequest = mapper.Map(subQuotaRequestDTO, subQuotaRequest);

            subQuotaRequestRepository.Update(updatedSubQuotaRequest);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<SubQuotaRequestResponseDTO>(updatedSubQuotaRequest);
            return new ResponseWrapper<SubQuotaRequestResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<SubQuotaRequestResponseDTO>> Delete(CancellationToken cancellationToken,
            long id)
        {
            SubQuotaRequest subQuotaRequest = await subQuotaRequestRepository.GetByIdAsync(cancellationToken, id);

            subQuotaRequestRepository.SoftDelete(subQuotaRequest);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<SubQuotaRequestResponseDTO> { Result = true };
        }
    }
}