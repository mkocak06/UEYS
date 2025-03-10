using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdvisorThesisService
    {
        Task<ResponseWrapper<List<AdvisorThesisResponseDTO>>> GetListByThesisIdAsync(CancellationToken cancellationToken, long thesisId);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> PostAsync(CancellationToken cancellationToken, AdvisorThesisDTO advisorThesis);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> Delete(CancellationToken cancellationToken, long id, ChangeCoordinatorAdvisorThesisDTO? advisorThesisToDelete = null);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> Put(CancellationToken cancellationToken, long id, AdvisorThesisDTO advisorThesisDTO);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> ChangeCoordinatorAdvisor(CancellationToken cancellationToken, long id, ChangeCoordinatorAdvisorThesisDTO advisorThesisDTO);
        Task<ResponseWrapper<AdvisorThesisResponseDTO>> AddNotEducatorAdvisor(CancellationToken cancellationToken, AdvisorThesisDTO advisorThesisDTO);

    }
}
