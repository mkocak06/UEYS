using Shared.ResponseModels.ENabiz;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces;

public interface IENabizService
{
    Task<ResponseWrapper<List<StudentResponseDTO>>> StudentList(CancellationToken cancellationToken,
        DateTime? createDate);
    Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> ExpertiseBranchList(CancellationToken cancellationToken);
}