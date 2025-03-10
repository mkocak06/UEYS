using Microsoft.JSInterop;
using Shared.ResponseModels.Ekip;
using Shared.ResponseModels.ENabizPortfolio;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UI.Services
{
    public interface ISpecialistDoctorService
    {
        Task<ResponseWrapper<List<PersonelResponseDTO>>> GetListByProgramId(long programId);
    }

    public class SpecialistDoctorService : ISpecialistDoctorService
    {
        private readonly IHttpService _httpService;

        public SpecialistDoctorService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<List<PersonelResponseDTO>>> GetListByProgramId(long programId)
        {
            return await _httpService.Get<ResponseWrapper<List<PersonelResponseDTO>>>($"SpecialistDoctor/GetListByProgramId/{programId}");
        }
    }
}
