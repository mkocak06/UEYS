using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace Application.Interfaces
{
    public interface IEthicCommitteeDecisionService
    {
        Task<PaginationModel<EthicCommitteeDecisionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> PostAsync(CancellationToken cancellationToken, EthicCommitteeDecisionDTO ethicCommitteeDecisionDTO);
        Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Put(CancellationToken cancellationToken, long id, EthicCommitteeDecisionDTO ethicCommitteeDecisionDTO);
        Task<ResponseWrapper<EthicCommitteeDecisionResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<EthicCommitteeDecisionResponseDTO>>> GetListByThesisId(CancellationToken cancellationToken, long thesisId);
    }
}
