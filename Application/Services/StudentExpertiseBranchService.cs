using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Services
{
    public class StudentExpertiseBranchService : BaseService, IStudentExpertiseBranchService
    {

        private readonly IMapper mapper;
        private readonly IStudentExpertiseBranchRepository studentExpertiseBranchRepository;

        public StudentExpertiseBranchService(IMapper mapper, IUnitOfWork unitOfWork, IStudentExpertiseBranchRepository studentExpertiseBranchRepository) : base(unitOfWork)
        {
            this.mapper = mapper;
            this.studentExpertiseBranchRepository = studentExpertiseBranchRepository;
        }

        public async Task<ResponseWrapper<List<StudentExpertiseBranchResponseDTO>>> GetListByStudentIdAsync(CancellationToken cancellationToken, long studentId)
        {
            List<StudentExpertiseBranch> studentExpertiseBranch = await studentExpertiseBranchRepository.GetIncludingList(cancellationToken, x => x.StudentId == studentId, x => x.ExpertiseBranch);
            List<StudentExpertiseBranchResponseDTO> response = mapper.Map<List<StudentExpertiseBranchResponseDTO>>(studentExpertiseBranch);

            return new ResponseWrapper<List<StudentExpertiseBranchResponseDTO>> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentExpertiseBranchResponseDTO>> PostAsync(CancellationToken cancellationToken, StudentExpertiseBranchDTO studentExpertiseBranchDTO)
        {
            StudentExpertiseBranch studentExpertiseBranch = mapper.Map<StudentExpertiseBranch>(studentExpertiseBranchDTO);

            await studentExpertiseBranchRepository.AddAsync(cancellationToken, studentExpertiseBranch);
            await unitOfWork.CommitAsync(cancellationToken);

            var response = mapper.Map<StudentExpertiseBranchResponseDTO>(studentExpertiseBranch);

            return new ResponseWrapper<StudentExpertiseBranchResponseDTO> { Result = true, Item = response };
        }

        public async Task<ResponseWrapper<StudentExpertiseBranchResponseDTO>> Delete(CancellationToken cancellationToken, long studentId, long expBranchId)
        {
            StudentExpertiseBranch studentExpertiseBranch = await studentExpertiseBranchRepository.GetByAsync(cancellationToken, x => x.StudentId == studentId && x.ExpertiseBranchId == expBranchId);

            studentExpertiseBranchRepository.HardDelete(studentExpertiseBranch);
            await unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<StudentExpertiseBranchResponseDTO> { Result = true };
        }
    }
}
