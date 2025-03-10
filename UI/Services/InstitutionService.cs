using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.StatisticModels;
using Shared.ResponseModels.Student;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IInstitutionService
{
    Task<ResponseWrapper<List<InstitutionResponseDTO>>> GetAll();
    Task<ResponseWrapper<InstitutionResponseDTO>> GetById(long id);
    Task<ResponseWrapper<InstitutionResponseDTO>> Add(InstitutionDTO institution);
    Task<ResponseWrapper<InstitutionResponseDTO>> Update(long id, InstitutionDTO institution);
    Task<PaginationModel<InstitutionResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> GetCountsByParentInstitution(FilterDTO filter);
    Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> GetUniversityHospitalCountsByParentInstitution();
    Task Delete(long id);
}

public class InstitutionService : IInstitutionService
{
    private readonly IHttpService _httpService;

    public InstitutionService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> GetCountsByParentInstitution(FilterDTO filter)
    {
        return await _httpService.Post<ResponseWrapper<List<CountsByParentInstitutionModel>>>("Institution/GetCountsByParentInstitution", filter);
    }
    public async Task<ResponseWrapper<List<CountsByParentInstitutionModel>>> GetUniversityHospitalCountsByParentInstitution()
    {
        return await _httpService.Get<ResponseWrapper<List<CountsByParentInstitutionModel>>>("Institution/GetUniversityHospitalCountsByParentInstitution");
    }
    public async Task<ResponseWrapper<List<InstitutionResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<InstitutionResponseDTO>>>("Institution/GetList");
    }

    public async Task<ResponseWrapper<InstitutionResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<InstitutionResponseDTO>>($"Institution/Get/{id}");
    }

    public async Task<ResponseWrapper<InstitutionResponseDTO>> Add(InstitutionDTO institution)
    {
        return await _httpService.Post<ResponseWrapper<InstitutionResponseDTO>>($"Institution/Post", institution);
    }

    public async Task<ResponseWrapper<InstitutionResponseDTO>> Update(long id, InstitutionDTO institution)
    {
        return await _httpService.Put<ResponseWrapper<InstitutionResponseDTO>>($"Institution/Put/{id}", institution);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"Institution/Delete/{id}");
    }

    public async Task<PaginationModel<InstitutionResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<InstitutionResponseDTO>>($"Institution/GetPaginateList", filter);
    }
}