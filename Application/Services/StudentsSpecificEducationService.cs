using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Extentsions;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
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
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Migrations;

namespace Application.Services
{
    public class StudentsSpecificEducationService : BaseService, IStudentsSpecificEducationService
    {
        private readonly IMapper mapper;
        private readonly IStudentsSpecificEducationRepository studentsSpecificEducationRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public StudentsSpecificEducationService(IMapper mapper, IUnitOfWork unitOfWork, IStudentsSpecificEducationRepository studentsSpecificEducationRepository, IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studentsSpecificEducationRepository = studentsSpecificEducationRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Delete(CancellationToken cancellationToken, long id)
        {
            StudentsSpecificEducation specificEducation = await studentsSpecificEducationRepository.GetByIdAsync(cancellationToken, id);
            studentsSpecificEducationRepository.SoftDelete(specificEducation);
            await unitOfWork.CommitAsync(cancellationToken);
            return new() { Result = true };
        }

        public async Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id)
        {

            StudentsSpecificEducation studentsSpecificEducation = await studentsSpecificEducationRepository.GetIncluding(cancellationToken, x => x.Id == id, x => x.SpecificEducation, x => x.SpecificEducationPlace);

            StudentSpecificEducationResponseDTO response = mapper.Map<StudentSpecificEducationResponseDTO>(studentsSpecificEducation);

            if (studentsSpecificEducation != null)
            {
                var docs = await unitOfWork.DocumentRepository.GetByEntityId(cancellationToken, id, Shared.Types.DocumentTypes.SpecificEducation);
                var docsResponse = mapper.Map<List<DocumentResponseDTO>>(docs);
                response.Documents = docsResponse;
            }
            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<StudentSpecificEducationResponseDTO>>> GetListAsync(CancellationToken cancellationToken)
        {
            IQueryable<StudentsSpecificEducation> query = studentsSpecificEducationRepository.IncludingQueryable(x => x.IsDeleted == false, x => x.SpecificEducation, x => x.SpecificEducationPlace);

            List<StudentsSpecificEducation> studentsSpecificEducations = await query.ToListAsync(cancellationToken);

            List<StudentSpecificEducationResponseDTO> response = mapper.Map<List<StudentSpecificEducationResponseDTO>>(studentsSpecificEducations);

            return new ResponseWrapper<List<StudentSpecificEducationResponseDTO>> { Result = true, Item = response };
        }

        public async Task<PaginationModel<StudentSpecificEducationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter)
        {
            IQueryable<StudentsSpecificEducation> ordersQuery = studentsSpecificEducationRepository.IncludingQueryable(x => true, x => x.SpecificEducationPlace, x => x.SpecificEducation);
            FilterResponse<StudentsSpecificEducation> filterResponse = ordersQuery.ToFilterView(filter);
            var studentsSpecificEducations = mapper.Map<List<StudentSpecificEducationResponseDTO>>(await filterResponse.Query.ToListAsync());
            var response = new PaginationModel<StudentSpecificEducationResponseDTO>
            {
                Items = studentsSpecificEducations,
                TotalPages = filterResponse.PageNumber,
                Page = filter.page,
                PageSize = filter.pageSize,
                TotalItemCount = filterResponse.Count
            };

            return response;
        }

        public async Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentSpecificEducationDTO studentsSpecificEducationDTO)
        {
            StudentsSpecificEducation studentsSpecificEducation = mapper.Map<StudentsSpecificEducation>(studentsSpecificEducationDTO);

            await studentsSpecificEducationRepository.AddAsync(cancellationToken, studentsSpecificEducation);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentSpecificEducationResponseDTO>(studentsSpecificEducation);

            return new() { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentSpecificEducationDTO studentsSpecificEducationDTO)
        {
            StudentsSpecificEducation studentsSpecificEducation = await studentsSpecificEducationRepository.GetByIdAsync(cancellationToken, id);
            StudentsSpecificEducation updatedStudentsSpecificEducation = mapper.Map(studentsSpecificEducationDTO, studentsSpecificEducation);
            studentsSpecificEducationRepository.Update(updatedStudentsSpecificEducation);
            await unitOfWork.CommitAsync(cancellationToken);
            var response = mapper.Map<StudentSpecificEducationResponseDTO>(updatedStudentsSpecificEducation);
            return new() { Result = true, Item = response };
        }
    }
}

