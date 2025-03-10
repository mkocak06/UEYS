using Shared.FilterModels.Base;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ResponseWrapper<UserResponseDTO>> GetUserByIdentityNoWithEducationalInfo(CancellationToken cancellationToken, string identityNo);
        Task<ResponseWrapper<UserResponseDTO>> PostWithEdu(CancellationToken cancellationToken, UserDTO userDTO);
        Task<ResponseWrapper<UserResponseDTO>> PostWithStu(CancellationToken cancellationToken, UserDTO userDTO);
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetUserByIdentityNoAsync(CancellationToken cancellationToken, string identityNo);
        Task<ResponseWrapper<bool>> IsExistingUser(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetById(CancellationToken cancellationToken, long id);
        ResponseWrapper<List<ActivePassiveResponseModel>> GetActivePassiveList();
        Task<bool> UpdateActivePassiveUser(CancellationToken cancellationToken, long userId);
        Task<ResponseWrapper<KPSResultResponseDTO>> GetUserInfoById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<AcademicAdminStaffResponseDTO>> GetAcademicInfoById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoById(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<GraduationDetailResponseDTO>>> GetGraduationInfoById(CancellationToken cancellationToken, long id);
        Task<bool> CheckEmailExist(CancellationToken cancellationToken, string email);
        Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoByIdentityNo(string identityNo);
        Task<ResponseWrapper<UserResponseDTO>> GetUserByIdentityNoForThesis(CancellationToken cancellationToken, string identityNo, bool forAdvisor);
        Task<ResponseWrapper<UserResponseDTO>> GetUserForChangeIdentityNoAndName(CancellationToken cancellationToken, string identityNo);
        Task<ResponseWrapper<UserResponseDTO>> ChangeIdentityNoAndName(CancellationToken cancellationToken, string identityNo, long id);
    }
}
