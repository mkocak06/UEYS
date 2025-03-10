using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.ResponseModels.StatisticModels;

namespace Application.Interfaces
{
    public interface IInstitutionService
    {
        Task<ResponseWrapper<List<InstitutionResponseDTO>>> GetListAsync(CancellationToken cancellationToken);
        Task<ResponseWrapper<InstitutionResponseDTO>> PostAsync(CancellationToken cancellationToken, InstitutionDTO institutionDTO);
        Task<ResponseWrapper<InstitutionResponseDTO>> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<ResponseWrapper<InstitutionResponseDTO>> Put(CancellationToken cancellationToken, long id, InstitutionDTO institutionDTO);
        Task<ResponseWrapper<InstitutionResponseDTO>> Delete(CancellationToken cancellationToken, long id);
        Task<PaginationModel<InstitutionResponseDTO>> GetPaginateList(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> CountsByParentInstitution(CancellationToken cancellationToken, FilterDTO filter);
        Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> UniversityHospitalCountsByParentInstitution(CancellationToken cancellationToken);

    }
}
