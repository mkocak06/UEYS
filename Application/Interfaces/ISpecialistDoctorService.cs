using Shared.ResponseModels.Ekip;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISpecialistDoctorService
    {
        Task<ResponseWrapper<List<PersonelResponseDTO>>> GetListByProgramId(CancellationToken cancellationToken, long programId);
    }
}
