using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.Models;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;

namespace UI.Services
{
    public interface IUserService
    {
        Task<ResponseWrapper<UserResponseDTO>> GetUserByIdentityNoWithEducationalInfo(string identityNo);
        Task<ResponseWrapper<KPSResultResponseDTO>> CheckKPS(long tcNo);
        Task<ResponseWrapper<UserResponseDTO>> AddUserWithEducatorInfo(UserDTO userDTO);
        Task<ResponseWrapper<UserResponseDTO>> AddUserWithStudentInfo(UserDTO userDTO);
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetUserByIdentityNo(string identityNo);
        Task<ResponseWrapper<UserResponseDTO>> GetUserForChangeIdentityNoAndName(string identityNo);
        Task<ResponseWrapper<UserResponseDTO>> ChangeIdentityNoAndName(string identityNo, long id);
        Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetById(long id);
        Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetActivePassiveForChart();
        Task<ResponseWrapper<KPSResultResponseDTO>> GetUserInfoById(long id);
        Task<ResponseWrapper<AcademicAdminStaffResponseDTO>> GetAcademicInfoById(long id);
        Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoById(long id);
        Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoByIdentityNo(string identityNo);
        Task<ResponseWrapper<List<GraduationDetailResponseDTO>>> GetGraduationInfoById(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
        Task<bool> UpdateActivePassiveUser(long userId);
        Task<bool> CheckEmailExist(string email);
        Task<ResponseWrapper<UserResponseDTO>> GetUserByIdentityNoForThesis(string identityNo, bool forAdvisor);
    }

    public class UserService : IUserService
    {
        private readonly IHttpService _httpService;

        public UserService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<UserResponseDTO>> GetUserByIdentityNoWithEducationalInfo(string identityNo)
        {
            return await _httpService.Get<ResponseWrapper<UserResponseDTO>>($"User/GetUserByIdentityNoWithEducationalInfo/{identityNo}");
        }
        public async Task<ResponseWrapper<UserResponseDTO>>  GetUserByIdentityNoForThesis(string identityNo, bool forAdvisor)
        {
            return await _httpService.Get<ResponseWrapper<UserResponseDTO>>($"User/GetUserByIdentityNoForThesis?identityNo={identityNo}&forAdvisor={forAdvisor}");
        }
        public async Task<bool> CheckEmailExist(string email)
        {
            return await _httpService.Get<bool>($"User/CheckEmailExist/{email}");
        }
        public async Task<ResponseWrapper<KPSResultResponseDTO>> CheckKPS(long tcNo)
        {
            return await _httpService.Get<ResponseWrapper<KPSResultResponseDTO>>($"User/CheckKPS?tcNo={tcNo}");
        }

        public async Task<ResponseWrapper<UserResponseDTO>> AddUserWithEducatorInfo(UserDTO userDTO)
        {
            return await _httpService.Post<ResponseWrapper<UserResponseDTO>>("User/AddUserWithEducatorInfo", userDTO);
        }

        

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<UserAccountDetailInfoResponseDTO>>($"User/GetById/{id}");
        }

        public async Task<ResponseWrapper<UserResponseDTO>> AddUserWithStudentInfo(UserDTO userDTO)
        {
            return await _httpService.Post<ResponseWrapper<UserResponseDTO>>("User/AddUserWithStudentInfo", userDTO);
        }

        public async Task<ResponseWrapper<UserWithStudentInfoResponseDTO>> GetWithStudentInfoByIdentityNo(string identityNo)
        {
            return await _httpService.Get<ResponseWrapper<UserWithStudentInfoResponseDTO>>($"User/GetWithStudentInfoByIdentityNo/{identityNo}");
        }

        public async Task<ResponseWrapper<UserAccountDetailInfoResponseDTO>> GetUserByIdentityNo(string identityNo)
        {
            return await _httpService.Get<ResponseWrapper<UserAccountDetailInfoResponseDTO>>($"User/GetUserByIdentityNo/{identityNo}");
        }

        public async Task<ResponseWrapper<UserResponseDTO>> GetUserForChangeIdentityNoAndName(string identityNo)
        {
            return await _httpService.Get<ResponseWrapper<UserResponseDTO>>($"User/GetUserForChangeIdentityNoAndName?identityNo={identityNo}");
        }

        public async Task<ResponseWrapper<UserResponseDTO>> ChangeIdentityNoAndName(string identityNo, long id)
        {
            return await _httpService.Get<ResponseWrapper<UserResponseDTO>>($"User/ChangeIdentityNoAndName?identityNo={identityNo}&id={id}");
        }

        public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetActivePassiveForChart()
        {
            return await _httpService.Get<ResponseWrapper<List<ActivePassiveResponseModel>>>($"User/GetActivePassiveForChart");
        }

        public async Task<ResponseWrapper<KPSResultResponseDTO>> GetUserInfoById(long id)
        {
            return await _httpService.Get<ResponseWrapper<KPSResultResponseDTO>>($"User/GetUserInfoById/{id}");
        }

        public async Task<ResponseWrapper<AcademicAdminStaffResponseDTO>> GetAcademicInfoById(long id)
        {
            return await _httpService.Get<ResponseWrapper<AcademicAdminStaffResponseDTO>>($"User/GetAcademicInfoById/{id}");
        }

        public async Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoById(long id)
        {
            return await _httpService.Get<ResponseWrapper<CKYSDoctor>>($"User/GetCKYSInfoById/{id}");
        }

        public async Task<ResponseWrapper<CKYSDoctor>> GetCKYSInfoByIdentityNo(string identityNo)
        {
            return await _httpService.Get<ResponseWrapper<CKYSDoctor>>($"User/GetCKYSInfoByIdentityNo/{identityNo}");
        }

        public async Task<ResponseWrapper<List<GraduationDetailResponseDTO>>> GetGraduationInfoById(long id)
        {
            return await _httpService.Get<ResponseWrapper<List<GraduationDetailResponseDTO>>>($"User/GetGraduationInfoById/{id}");
        }

        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"User/DownloadFile/{bucketKey}");
        }
        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"User/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }

        public async Task<bool> UpdateActivePassiveUser(long userId)
        {
           return await _httpService.Put<bool>($"User/UpdateActivePassiveUser/{userId}", null);
        }
    }
}
