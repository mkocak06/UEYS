using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IEducatorProgramService
    {
        Task<ResponseWrapper<EducatorProgramResponseDTO>> Add(EducatorProgramDTO educatorProgram);
        Task<ResponseWrapper<EducatorProgramResponseDTO>> Update(long id, EducatorProgramDTO educatorProgram);
        Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByProgramId(long programId);
        Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByHospitalId(long hospitalId);
        Task<ResponseWrapper<EducatorProgramResponseDTO>> Delete(long id);
        Task<ResponseWrapper<EducatorProgramResponseDTO>> GetById(long? id);
    }
    public class EducatorProgramService : IEducatorProgramService
    {
        private readonly IHttpService _httpService;

        public EducatorProgramService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> Add(EducatorProgramDTO educatorProgram)
        {
            return await _httpService.Post<ResponseWrapper<EducatorProgramResponseDTO>>($"EducatorProgram/Post", educatorProgram);
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> Update(long id, EducatorProgramDTO educatorProgram)
        {
            return await _httpService.Put<ResponseWrapper<EducatorProgramResponseDTO>>($"EducatorProgram/Put/{id}", educatorProgram);
        }

        public async Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByProgramId(long programId)
        {
            return await _httpService.Get<ResponseWrapper<List<EducatorProgramResponseDTO>>>($"EducatorProgram/GetListByProgramId/{programId}");
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> Delete(long id)
        {
            return await _httpService.Get<ResponseWrapper<EducatorProgramResponseDTO>>($"EducatorProgram/Delete?id={id}");
        }

        public async Task<ResponseWrapper<EducatorProgramResponseDTO>> GetById(long? id)
        {
            return await _httpService.Get<ResponseWrapper<EducatorProgramResponseDTO>>($"EducatorProgram/GetById?id={id}");
        }

        public async Task<ResponseWrapper<List<EducatorProgramResponseDTO>>> GetListByHospitalId(long hospitalId)
        {
            return await _httpService.Get<ResponseWrapper<List<EducatorProgramResponseDTO>>>($"EducatorProgram/GetListByHospitalId/{hospitalId}");
        }
    }
}
