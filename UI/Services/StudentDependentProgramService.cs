using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.ProtocolProgram;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IStudentDependentProgramService
    {
        Task<ResponseWrapper<StudentDependentProgramResponseDTO>> Update(long id, StudentDependentProgramDTO StudentDependentProgram);
        Task<ResponseWrapper<List<StudentDependentProgramPaginateDTO>>> GetListByStudentId(long studentId);
    }
    public class StudentDependentProgramService : IStudentDependentProgramService
    {
        private readonly IHttpService _httpService;

        public StudentDependentProgramService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseWrapper<List<StudentDependentProgramPaginateDTO>>> GetListByStudentId(long studentId)
        {
            return await _httpService.Get<ResponseWrapper<List<StudentDependentProgramPaginateDTO>>>($"StudentDependentProgram/GetListByStudentId?studentId={studentId}");
        }

        public async Task<ResponseWrapper<StudentDependentProgramResponseDTO>> Update(long id, StudentDependentProgramDTO StudentDependentProgram)
        {
            return await _httpService.Put<ResponseWrapper<StudentDependentProgramResponseDTO>>($"StudentDependentProgram/Put/{id}", StudentDependentProgram);
        }
       
    }
}
