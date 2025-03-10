using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Types;
using System;
using System.Xml.Linq;

namespace UI.Services
{
    public interface IThesisDefenceService
    {
        Task<PaginationModel<ThesisDefenceResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> GetById(long id);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> Add(ThesisDefenceDTO ThesisDefence);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> Update(long id, ThesisDefenceDTO ThesisDefence);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> IsThesisDefenceAddable(long thesisId, DateTime? date);
        Task<ResponseWrapper<ThesisDefenceResponseDTO>> Delete(long id, long studentId);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class ThesisDefenceService : IThesisDefenceService
    {
        private readonly IHttpService _httpService;

        public ThesisDefenceService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> Add(ThesisDefenceDTO ThesisDefence)
        {
            return await _httpService.Post<ResponseWrapper<ThesisDefenceResponseDTO>>($"ThesisDefence/Post", ThesisDefence);
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<ThesisDefenceResponseDTO>>($"ThesisDefence/GetById?id={id}");
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> Delete(long id, long studentId)
        {
            return await _httpService.Get<ResponseWrapper<ThesisDefenceResponseDTO>>($"ThesisDefence/Delete?id={id}&studentId={studentId}");
        }

        public async Task<PaginationModel<ThesisDefenceResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<ThesisDefenceResponseDTO>>($"ThesisDefence/GetPaginateList", filter);
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> Update(long id, ThesisDefenceDTO ThesisDefence)
        {
            return await _httpService.Put<ResponseWrapper<ThesisDefenceResponseDTO>>($"ThesisDefence/Put/{id}", ThesisDefence);
        }

        public async Task<ResponseWrapper<ThesisDefenceResponseDTO>> IsThesisDefenceAddable(long thesisId, DateTime? date)
        {
            return await _httpService.Get<ResponseWrapper<ThesisDefenceResponseDTO>>($"ThesisDefence/IsThesisDefenceAddable?thesisId={thesisId}&date={date}");
        }

        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"ThesisDefence/DownloadFile/{bucketKey}");
        }

        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"ThesisDefence/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}
