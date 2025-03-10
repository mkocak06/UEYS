using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Threading.Tasks;

namespace UI.Services
{
    public interface IEducatorExpertiseBranchService
    {
        Task<ResponseWrapper<EducatorExpertiseBranchResponseDTO>> AddEducatorExpBranch(EducatorExpertiseBranchDTO educatorExpertiseBranchDTO);
        Task RemoveEducatorExpBranch(long id);
    }

    public class EducatorExpertiseBranchService : IEducatorExpertiseBranchService
    {
        private readonly IHttpService _httpService;

        public EducatorExpertiseBranchService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<EducatorExpertiseBranchResponseDTO>> AddEducatorExpBranch(EducatorExpertiseBranchDTO educatorExpertiseBranchDTO)
        {
            return await _httpService.Post<ResponseWrapper<EducatorExpertiseBranchResponseDTO>>($"EducatorExpertiseBranch/PostAsync", educatorExpertiseBranchDTO);
        }

        public async Task RemoveEducatorExpBranch(long id)
        {
            await _httpService.Delete($"EducatorExpertiseBranch/Delete/{id}");
        }
    }
}
