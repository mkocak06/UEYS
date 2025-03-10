using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Educator;
using Shared.ResponseModels.Hospital;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Wrapper;
using UI.Models.FilterModels;


namespace UI.Services
{

    public interface IHospitalService
    {
        Task<ResponseWrapper<List<HospitalResponseDTO>>> GetAll();
        Task<PaginationModel<HospitalResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<PaginationModel<HospitalResponseDTO>> GetArchiveList(FilterDTO filter);
        Task<ResponseWrapper<List<HospitalResponseDTO>>> GetListByUniversityId(long uniId);
        Task<ResponseWrapper<List<HospitalBreadcrumbDTO>>> GetListByExpertiseBranchId(long uniId);
        Task<ResponseWrapper<HospitalResponseDTO>> GetById(long id);
        Task<ResponseWrapper<HospitalResponseDTO>> Add(HospitalDTO hospital);
        Task<ResponseWrapper<HospitalResponseDTO>> Update(long id, HospitalDTO hospital);
        Task<ResponseWrapper<HospitalResponseDTO>> Undelete(long id);
        Task Delete(long id);
        Task<ResponseWrapper<UserHospitalDetailDTO>> GetUserHospitalDetail();
        Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter);
        Task<ResponseWrapper<byte[]>> GetDetailedExcelByteArray(ProgramFilter filter);
        Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetHospitalCountByParentInstitution(FilterDTO filter);
    }

    public class HospitalService : IHospitalService
    {
        private readonly IHttpService _httpService;

        public HospitalService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<List<HospitalResponseDTO>>> GetAll()
        {
            return await _httpService.Get<ResponseWrapper<List<HospitalResponseDTO>>>("Hospital/GetList");
        }

        public async Task<PaginationModel<HospitalResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<HospitalResponseDTO>>("Hospital/GetPaginateList", filter);
        }

        public async Task<PaginationModel<HospitalResponseDTO>> GetArchiveList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<HospitalResponseDTO>>("Archive/GetHospitalList", filter);
        }

        public async Task<ResponseWrapper<List<HospitalResponseDTO>>> GetListByUniversityId(long uniId)
        {
            return await _httpService.Get<ResponseWrapper<List<HospitalResponseDTO>>>($"Hospital/GetListByUniversityId/{uniId}");
        }

        public async Task<ResponseWrapper<List<HospitalBreadcrumbDTO>>> GetListByExpertiseBranchId(long expId)
        {
            return await _httpService.Get<ResponseWrapper<List<HospitalBreadcrumbDTO>>>($"Hospital/GetListByExpertiseBranchId/{expId}");
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<HospitalResponseDTO>>($"Hospital/Get/{id}");
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> Add(HospitalDTO hospital)
        {
            return await _httpService.Post<ResponseWrapper<HospitalResponseDTO>>($"Hospital/Post", hospital);
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> Update(long id, HospitalDTO hospital)
        {
            return await _httpService.Put<ResponseWrapper<HospitalResponseDTO>>($"Hospital/Put/{id}", hospital);
        }

        public async Task<ResponseWrapper<HospitalResponseDTO>> Undelete(long id)
        {
            return await _httpService.Put<ResponseWrapper<HospitalResponseDTO>>($"Archive/UnDeleteHospital/{id}", null);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"Hospital/Delete/{id}");
        }

        public async Task<ResponseWrapper<UserHospitalDetailDTO>> GetUserHospitalDetail()
        {
            return await _httpService.Get<ResponseWrapper<UserHospitalDetailDTO>>($"Hospital/GetUserHospitalDetail");
        }

		public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter)
		{
			return await _httpService.Post<ResponseWrapper<byte[]>>("Hospital/ExcelExport", filter);
		}
        public async Task<ResponseWrapper<byte[]>> GetDetailedExcelByteArray(ProgramFilter filter)
        {
            return await _httpService.ExcelByteArray("Hospital/DetailedExcelExport", filter);
        }
        public async Task<ResponseWrapper<List<ActivePassiveResponseModel>>> GetHospitalCountByParentInstitution(FilterDTO filter)
        {
            return await _httpService.Post<ResponseWrapper<List<ActivePassiveResponseModel>>>("Hospital/GetHospitalCountByParentInstitution", filter);
        }
    }
}
