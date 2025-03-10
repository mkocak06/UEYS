using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Types;

namespace UI.Services
{
    public interface IEthicCommitteeService
    {
        Task<PaginationModel<EthicCommitteeDecisionResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Add(EthicCommitteeDecisionDTO EthicCommittee);
        Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Update(long id, EthicCommitteeDecisionDTO EthicCommittee);
        Task Delete(long id);
        Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey);
        Task DeleteFile(DocumentTypes documentType, long entityId);
    }

    public class EthicCommitteeService : IEthicCommitteeService
    {
        private readonly IHttpService _httpService;

        public EthicCommitteeService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Add(EthicCommitteeDecisionDTO EthicCommittee)
        {
            return await _httpService.Post<ResponseWrapper<EthicCommitteeDecisionResponseDTO>>($"EthicCommitteeDecision/Post", EthicCommittee);
        }

        public async Task Delete(long id)
        {
            await _httpService.Delete($"EthicCommitteeDecision/Delete/{id}");
        }

        public async Task<PaginationModel<EthicCommitteeDecisionResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<EthicCommitteeDecisionResponseDTO>>($"EthicCommitteeDecision/GetPaginateList", filter);
        }

        public async Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Update(long id, EthicCommitteeDecisionDTO EthicCommittee)
        {
            return await _httpService.Put<ResponseWrapper<EthicCommitteeDecisionResponseDTO>>($"EthicCommitteeDecision/Put/{id}", EthicCommittee);
        }
        public async Task<ResponseWrapper<FileResponseDTO>> Download(string bucketKey)
        {
            return await _httpService.Get<ResponseWrapper<FileResponseDTO>>($"EthicCommitteeDecision/DownloadFile/{bucketKey}");
        }
        public async Task DeleteFile(DocumentTypes documentType, long entityId)
        {
            await _httpService.Delete($"EthicCommitteeDecision/DeleteFileS3?documentType={documentType}&entityId={entityId}");
        }
    }
}
