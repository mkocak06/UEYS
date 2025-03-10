using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Infrastructure.Data;
using Shared.FilterModels.Base;
using Shared.FilterModels;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class LogService : BaseService, ILogService
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _logRepository;
        private readonly IUserRepository _userRepository;

        public LogService(IUnitOfWork unitOfWork, ILogRepository logRepository, IMapper mapper, IUserRepository userRepository) : base(unitOfWork)
        {
            _logRepository = logRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        public async Task<PaginationModel<LogResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<Log> ordersQuery = unitOfWork.LogRepository.Queryable();
            FilterResponse<Log> filterResponse = ordersQuery.ToFilterView(filter);

            var logs = _mapper.Map<List<LogResponseDTO>>(await filterResponse.Query.ToListAsync(cancellationToken));

            var response = new PaginationModel<LogResponseDTO>
            {
                Items = logs,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<LogResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            Log log = await _logRepository.GetByIdAsync(cancellationToken, id);
            LogResponseDTO response = _mapper.Map<LogResponseDTO>(log);
            if (log.UserId != null && log.UserId != 0)
            {
                var user = await _userRepository.FirstOrDefaultAsync(cancellationToken, x => x.Id == log.UserId);
                response.User = _mapper.Map<UserResponseDTO>(user);
            }

            return new ResponseWrapper<LogResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<LogResponseDTO>> PostAsync(CancellationToken cancellationToken, LogDTO logDTO)
        {
            Log log = _mapper.Map<Log>(logDTO);

            await _logRepository.AddAsync(cancellationToken, log);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = _mapper.Map<LogResponseDTO>(log);

            return new ResponseWrapper<LogResponseDTO> { Result = true, Item = response };
        }

    }
}
