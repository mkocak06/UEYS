using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAffiliationService
    {
        Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<PaginationModel<AffiliationResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByUniversityId(CancellationToken cancellationToken, long uniId);
        Task<ResponseWrapper<List<AffiliationResponseDTO>>> GetListByHospitalId(CancellationToken cancellationToken, long hospitalId);
        Task<ResponseWrapper<AffiliationResponseDTO>> PostAsync(CancellationToken cancellationToken, AffiliationDTO educatorDTO);
        Task<ResponseWrapper<AffiliationResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id, DocumentTypes docType);
        Task<ResponseWrapper<AffiliationResponseDTO>> GetByAffiliation(CancellationToken cancellationToken, long facultyid, long hospitalid);
        Task<ResponseWrapper<AffiliationResponseDTO>> Put(CancellationToken cancellationToken, long id, AffiliationDTO educatorDTO);
        Task<ResponseWrapper<AffiliationResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<byte[]>> ExcelExport(CancellationToken cancellationToken, FilterDTO filter);
	}
}
