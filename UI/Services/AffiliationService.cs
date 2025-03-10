using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Helper;

namespace UI.Services
{
    public interface IAffiliationService
    {
        Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetAll();
        Task<PaginationModel<AffiliationResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByUniversityId(long uniId);
        Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByHospitalId(long id);
        Task<ResponseWrapper<AffiliationResponseDTO>> GetById(long id, DocumentTypes docType);
        Task<ResponseWrapper<AffiliationResponseDTO>> Add(AffiliationDTO affiliation);
        Task<ResponseWrapper<AffiliationResponseDTO>> Update(long id, AffiliationDTO affiliation);
        Task<ResponseWrapper<AffiliationResponseDTO>> GetByAffiliation(long facultyid, long hospitalid);
        Task Delete(long id);
        Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class AffiliationService : IAffiliationService
    {
        private readonly IHttpService _httpService;

        public AffiliationService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetAll()
        {
            return await _httpService.Get<ResponseWrapper<List<AffiliationResponseDTO>>>("Affiliation/GetList");
        }
        public async Task<ResponseWrapper<AffiliationResponseDTO>> GetById(long id, DocumentTypes docType)
        {
            return await _httpService.Get<ResponseWrapper<AffiliationResponseDTO>>($"Affiliation/Get?id={id}&docType={docType}");
        }
        public async Task<ResponseWrapper<AffiliationResponseDTO>> GetByAffiliation(long facultyid,long hospitalid)
        {
            return await _httpService.Get<ResponseWrapper<AffiliationResponseDTO>>($"Affiliation/GetByAffiliation?facultyId={facultyid}&hospitalId={hospitalid}");
        }
        public async Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByUniversityId(long uniId)
        {
            return await _httpService.Get<ResponseWrapper<List<AffiliationResponseDTO>>>($"Affiliation/GetListByUniversityId/{uniId}");
        }
        public async Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByHospitalId(long hospitalId)
        {
            return await _httpService.Get<ResponseWrapper<List<AffiliationResponseDTO>>>($"Affiliation/GetListByHospitalId/{hospitalId}");
        }
        public async Task<PaginationModel<AffiliationResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<AffiliationResponseDTO>>("Affiliation/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<AffiliationResponseDTO>> Add(AffiliationDTO affiliation)
        {
            return await _httpService.Post<ResponseWrapper<AffiliationResponseDTO>>($"Affiliation/Post", affiliation);
        }
        public async Task<ResponseWrapper<AffiliationResponseDTO>> Update(long id, AffiliationDTO affiliation)
        {
            return await _httpService.Put<ResponseWrapper<AffiliationResponseDTO>>($"Affiliation/Put/{id}", affiliation);
        }
        public async Task Delete(long id)
        {
            await _httpService.Delete($"Affiliation/Delete/{id}");
        }
		public async Task<ResponseWrapper<byte[]>> GetExcelByteArray(FilterDTO filter)
		{
			return await _httpService.Post<ResponseWrapper<byte[]>>("Affiliation/ExcelExport", filter);
		}

        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"Affiliation/DownloadFile/{bucketKey}");
        }

        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"Affiliation/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}
