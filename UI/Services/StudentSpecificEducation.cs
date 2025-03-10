using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services
{
    public interface IStudentSpecificEducationService
    {
        Task<PaginationModel<StudentSpecificEducationResponseDTO>> GetPaginateList(FilterDTO filter);
        Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> GetById(long studentId);
        Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Update(long id, StudentSpecificEducationDTO studentSpecificEducation);
        Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Add(StudentSpecificEducationDTO studentSpecificEducation);
        Task Delete(long id);

    }

    public class StudentSpecificEducationService : IStudentSpecificEducationService
    {
        private readonly IHttpService _httpService;

        public StudentSpecificEducationService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<PaginationModel<StudentSpecificEducationResponseDTO>> GetPaginateList(FilterDTO filter)
        {
            return await _httpService.Post<PaginationModel<StudentSpecificEducationResponseDTO>>("StudentsSpecificEducation/GetPaginateList", filter);
        }
        public async Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> GetById(long id)
        {
            return await _httpService.Get<ResponseWrapper<StudentSpecificEducationResponseDTO>>($"StudentsSpecificEducation/Get/{id}");
        }
        public async Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Update(long id, StudentSpecificEducationDTO studentSpecificEducation)
        {
            return await _httpService.Put<ResponseWrapper<StudentSpecificEducationResponseDTO>>($"StudentsSpecificEducation/Put/{id}", studentSpecificEducation);
        }
        public async Task<ResponseWrapper<StudentSpecificEducationResponseDTO>> Add(StudentSpecificEducationDTO studentSpecificEducation)
        {
            return await _httpService.Post<ResponseWrapper<StudentSpecificEducationResponseDTO>>($"StudentsSpecificEducation/Post", studentSpecificEducation);
        }
        public async Task Delete(long id)
        {
            await _httpService.Delete($"StudentsSpecificEducation/Delete/{id}");
        }
    }
}

