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
    public class StudentCountService : BaseService, IStudentCountService
    {
        private readonly IMapper mapper;
        private readonly IStudentCountRepository studentCountRepository;

        public StudentCountService(IMapper mapper, IUnitOfWork unitOfWork, IStudentCountRepository studentCountRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studentCountRepository = studentCountRepository;
        }

        public async Task<PaginationModel<StudentCountResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<StudentCount> ordersQuery = studentCountRepository.IncludingQueryable(x => true, x => x.SubQuotaRequest.Program);
            FilterResponse<StudentCount> filterResponse = ordersQuery.ToFilterView(filter);

            var studentCounts = mapper.Map<List<StudentCountResponseDTO>>(await filterResponse.Query.ToListAsync());

            var response = new PaginationModel<StudentCountResponseDTO>
            {
                Items = studentCounts,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<StudentCountResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            StudentCount studentCount = await studentCountRepository.GetByAsync(cancellationToken, x => x.IsDeleted == false && x.Id == id);

            return new ResponseWrapper<StudentCountResponseDTO> { Result = true, Item = mapper.Map<StudentCountResponseDTO>(studentCount) };
        }

        public async Task<ResponseWrapper<StudentCountResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentCountDTO studentCountDTO)
        {
            StudentCount studentCount = mapper.Map<StudentCount>(studentCountDTO);

            await studentCountRepository.AddAsync(cancellationToken, studentCount);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentCountResponseDTO> { Result = true, Item = mapper.Map<StudentCountResponseDTO>(studentCount) };
        }

        public async Task<ResponseWrapper<StudentCountResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentCountDTO studentCountDTO)
        {
            StudentCount studentCount = await studentCountRepository.GetByIdAsync(cancellationToken, id);
            var updatedStudentCount = mapper.Map(studentCountDTO, studentCount);

            studentCountRepository.Update(updatedStudentCount);

            var response = mapper.Map<StudentCountResponseDTO>(studentCount);

            return new ResponseWrapper<StudentCountResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentCountResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            StudentCount studentCount = await studentCountRepository.GetByIdAsync(cancellationToken, id);

            studentCountRepository.SoftDelete(studentCount);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentCountResponseDTO> { Result = true };
        }
    }
}
