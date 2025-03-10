using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.FilterModels;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly IMapper mapper;
        private readonly INotificationRepository notificationRepository;
        public NotificationService(IUnitOfWork unitOfWork, INotificationRepository notificationRepository, IMapper mapper) : base(unitOfWork)
        {
            this.notificationRepository = notificationRepository;
            this.mapper = mapper;
        }

        public async Task<PaginationModel<NotificationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Notification> ordersQuery = notificationRepository.IncludingQueryable(x => true);

            FilterResponse<Notification> filterResponse = ordersQuery.ToFilterView(filter);

            var notifications = mapper.Map<List<NotificationResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            return new PaginationModel<NotificationResponseDTO>
            {
                Items = notifications,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };
        }

        public async Task<ResponseWrapper<NotificationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Notification notification = await notificationRepository.GetByIdAsync(cancellationToken, id);

            NotificationResponseDTO response = mapper.Map<NotificationResponseDTO>(notification);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<NotificationResponseDTO>> PostAsync(CancellationToken cancellationToken, NotificationDTO notificationDTO)
        {
            Notification notification = mapper.Map<Notification>(notificationDTO);

            await notificationRepository.AddAsync(cancellationToken, notification);
            await unitOfWork.CommitAsync(cancellationToken);

            NotificationResponseDTO response = mapper.Map<NotificationResponseDTO>(notification);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<NotificationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            Notification notification = await notificationRepository.GetByIdAsync(cancellationToken, id);

            notificationRepository.SoftDelete(notification);
            await unitOfWork.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
    }
}
