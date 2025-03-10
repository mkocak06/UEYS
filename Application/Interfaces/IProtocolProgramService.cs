using Amazon.Runtime.Documents;
using Microsoft.AspNetCore.Http;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Threading;

namespace Application.Interfaces
{
    public interface IProtocolProgramService
    {
        Task<ResponseWrapper<List<ProtocolProgramResponseDTO>>> GetListAsync(CancellationToken cancellationToken, ProgramType progType);
        Task<PaginationModel<ProtocolProgramPaginatedResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> PostAsync(CancellationToken cancellationToken, ProtocolProgramDTO protocolProtocolProgramDTO);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id, DocumentTypes docType, ProgramType progType);
        Task<ResponseWrapper<List<ProtocolProgramByUniversityIdResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId, ProgramType progType);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> GetByProgramId(CancellationToken cancellationToken, long programId, ProgramType progType);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> Put(CancellationToken cancellationToken, long id, ProtocolProgramResponseDTO protocolProtocolProgramDTO);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<ProtocolProgramResponseDTO>> UnDelete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<List<EducatorDependentProgramResponseDTO>>> GetEducatorListForComplementProgram(CancellationToken cancellationToken, long programId);

    }
}
