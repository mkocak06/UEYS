using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class UserNotificationService :BaseService, IUserNotificationService
    {
        private readonly IUserNotificationRepository userNotificationRepository;
        private readonly IMapper mapper;

        public UserNotificationService(IUserNotificationRepository userNotificationRepository, IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.userNotificationRepository = userNotificationRepository;
            this.mapper = mapper;
        }

        public async Task<PaginationModel<UserNotificationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            var ordersQuery = userNotificationRepository.IncludingQueryable(x => true,x=>x.Notification);

            var filterResponse = ordersQuery.ToFilterView(filter);

            var userNotifications = mapper.Map<List<UserNotificationResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            return new PaginationModel<UserNotificationResponseDTO>
            {
                Items = userNotifications,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
        }

        public async Task<ResponseWrapper<UserNotificationResponseDTO>> PostAsync(CancellationToken cancellationToken, UserNotificationDTO userNotificationDTO)
        {
            var userNotification = mapper.Map<UserNotification>(userNotificationDTO);

            await userNotificationRepository.AddAsync(cancellationToken, userNotification);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true, Item = mapper.Map<UserNotificationResponseDTO>(userNotification) };
        }

        public async Task<ResponseWrapper<UserNotificationResponseDTO>> Put(CancellationToken cancellationToken, long id, UserNotificationDTO userNotificationDTO)
        {
            var userNotification = await userNotificationRepository.GetByIdAsync(cancellationToken, id);

            var updatedUserNotification = mapper.Map(userNotificationDTO,userNotification);

            userNotificationRepository.Update(updatedUserNotification);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true, Item = mapper.Map<UserNotificationResponseDTO>(updatedUserNotification) };
        }

        public async Task<ResponseWrapper<UserNotificationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            var userNotification = await userNotificationRepository.GetByIdAsync(cancellationToken, id);

            userNotificationRepository.SoftDelete(userNotification);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}
