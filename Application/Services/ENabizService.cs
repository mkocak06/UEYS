using Application.Interfaces;
using Application.Services.Base;
using AutoMapper;
using Core.Interfaces;
using Core.UnitOfWork;
using Shared.ResponseModels.ENabiz;
using Shared.ResponseModels.Wrapper;

namespace Application.Services;

public class ENabizService : BaseService, IENabizService
{
    private readonly IMapper mapper;
    private readonly IStudentRepository studentRepository;
    private readonly IExpertiseBranchRepository expertiseBranchRepository;

    public ENabizService(IMapper mapper, IUnitOfWork unitOfWork, IStudentRepository studentRepository,
        IExpertiseBranchRepository expertiseBranchRepository) : base(unitOfWork)
    {
        this.mapper = mapper;
        this.studentRepository = studentRepository;
        this.expertiseBranchRepository = expertiseBranchRepository;
    }

    public async Task<ResponseWrapper<List<StudentResponseDTO>>> StudentList(CancellationToken cancellationToken, DateTime? createDate)
    {
        var students = await studentRepository.StudentListEnabiz(cancellationToken, createDate);

        List<StudentResponseDTO> response = mapper.Map<List<StudentResponseDTO>>(students);

        return new ResponseWrapper<List<StudentResponseDTO>> { Result = true, Item = response };
    }

    public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> ExpertiseBranchList(CancellationToken cancellationToken)
    {
        var expBranches = await expertiseBranchRepository.ExpertiseBranchListEnabiz(cancellationToken);

        var response = mapper.Map<List<ExpertiseBranchResponseDTO>>(expBranches);

        return new ResponseWrapper<List<ExpertiseBranchResponseDTO>> { Result = true, Item = response };
    }
}