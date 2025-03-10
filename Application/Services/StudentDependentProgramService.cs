using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Immutable;

namespace Application.Services
{
    public class StudentDependentProgramService : BaseService, IStudentDependentProgramService
    {
        private readonly IStudentDependentProgramRepository studentDependentProgramRepository;
        private readonly IMapper mapper;
        private readonly IDocumentRepository documentRepository;
        public StudentDependentProgramService(IUnitOfWork unitOfWork, IStudentDependentProgramRepository studentDependentProgramRepository, IMapper mapper, IDocumentRepository documentRepository) : base(unitOfWork)
        {
            this.studentDependentProgramRepository = studentDependentProgramRepository;
            this.mapper = mapper;
            this.documentRepository = documentRepository;
        }

        public async Task<ResponseWrapper<StudentDependentProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, StudentDependentProgramDTO studentDependentProgramDTO)
        {
            StudentDependentProgram studentDependentProgram = await studentDependentProgramRepository.GetByIdAsync(cancellationToken, id);

            StudentDependentProgram updatedStudentDependentProgram = mapper.Map(studentDependentProgramDTO, studentDependentProgram);

            studentDependentProgramRepository.Update(updatedStudentDependentProgram);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentDependentProgramResponseDTO>(updatedStudentDependentProgram);

            return new ResponseWrapper<StudentDependentProgramResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<List<StudentDependentProgramPaginateDTO>>> GetListByStudentId(CancellationToken cancellationToken, long studenId)
        {
            var ordersQuery = studentDependentProgramRepository.GetListByStudentId(studenId);
            var response = await ordersQuery.Select(x => new StudentDependentProgramPaginateDTO()
            {
                Id = x.Id,
                DependentProgramId = x.DependentProgramId,
                StudentId = x.StudentId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Duration = x.DependentProgram.Duration,
                Explanation = x.Explanation,
                IsActive = x.IsActive,
                IsCompleted = x.IsCompleted,
                IsUnCompleted = x.IsUnCompleted,
                RemainingDays = x.RemainingDays,
                ProgramName = x.DependentProgram.Program.Hospital.Province.Name + " - " + x.DependentProgram.Program.Hospital.Name + " - " + x.DependentProgram.Program.ExpertiseBranch.Name
            }).ToListAsync(cancellationToken);

            foreach (var item in response)
                item.Documents = mapper.Map<List<DocumentResponseDTO>>(await documentRepository.GetByEntityId(cancellationToken, (long)item.Id, DocumentTypes.StudentDependentProgram));

            return new() { Result = true, Item = response };
        }

    }
}
