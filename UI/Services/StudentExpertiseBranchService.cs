using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Threading.Tasks;

namespace UI.Services
{
    public interface IStudentExpertiseBranchService
    {
        Task<ResponseWrapper<StudentExpertiseBranchResponseDTO>> AddStudentExpBranch(StudentExpertiseBranchDTO studentExpertiseBranchDTO);
        Task RemoveStudentExpBranch(long studentId, long expBranchId);
    }

    public class StudentExpertiseBranchService : IStudentExpertiseBranchService
    {
        private readonly IHttpService _httpService;

        public StudentExpertiseBranchService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<StudentExpertiseBranchResponseDTO>> AddStudentExpBranch(StudentExpertiseBranchDTO studentExpertiseBranchDTO)
        {
            return await _httpService.Post<ResponseWrapper<StudentExpertiseBranchResponseDTO>>($"StudentExpertiseBranch/Post", studentExpertiseBranchDTO);
        }

        public async Task RemoveStudentExpBranch(long studentId, long expBranchId)
        {
            await _httpService.Delete($"StudentExpertiseBranch/Delete?studentId={studentId}&expBranchId={expBranchId}");
        }
    }
}
