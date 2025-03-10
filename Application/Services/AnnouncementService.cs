using Application.Interfaces;
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
    public class AnnouncementService : BaseService, IAnnouncementService
    {
        private readonly IMapper _mapper;
        private readonly IAnnouncementRepository _announcementRepository;

        public AnnouncementService(IUnitOfWork unitOfWork, IMapper mapper, IAnnouncementRepository announcementRepository) : base(unitOfWork)
        {
            _mapper = mapper;
            _announcementRepository = announcementRepository;
        }
        public async Task<PaginationModel<AnnouncementResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Announcement> ordersQuery = _announcementRepository.Get(x => !x.IsDeleted);
            FilterResponse<Announcement> filterResponse = ordersQuery.ToFilterView(filter);

            var announcements = _mapper.Map<List<AnnouncementResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<AnnouncementResponseDTO>
            {
                Items = announcements,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<AnnouncementResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Announcement announcement = await _announcementRepository.FirstOrDefaultAsync(cancellationToken, x => x.Id == id);

            AnnouncementResponseDTO response = _mapper.Map<AnnouncementResponseDTO>(announcement);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AnnouncementResponseDTO>> PostAsync(CancellationToken cancellationToken, AnnouncementDTO announcementDTO)
        {
            Announcement announcement = _mapper.Map<Announcement>(announcementDTO);

            await _announcementRepository.AddAsync(cancellationToken, announcement);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<AnnouncementResponseDTO>(announcement);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AnnouncementResponseDTO>> Put(CancellationToken cancellationToken, long id, AnnouncementDTO announcementDTO)
        {
            Announcement announcement = await _announcementRepository.GetByIdAsync(cancellationToken, id);

            Announcement updatedAnnouncement = _mapper.Map(announcementDTO, announcement);

            _announcementRepository.Update(updatedAnnouncement);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<AnnouncementResponseDTO>(updatedAnnouncement);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<AnnouncementResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Announcement announcement = await _announcementRepository.GetByIdAsync(cancellationToken, id);

            _announcementRepository.SoftDelete(announcement);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}