using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IFacultyService
{
    Task<PaginationModel<FacultyResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<List<FacultyResponseDTO>>> GetAll();
    Task<ResponseWrapper<List<FacultyResponseDTO>>> GetListByUniversityId(long uniId);
}

public class FacultyService : IFacultyService
{
    private readonly IHttpService _httpService;

    public FacultyService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<PaginationModel<FacultyResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<FacultyResponseDTO>>($"Faculty/GetPaginateList", filter);
    }

    public async Task<ResponseWrapper<List<FacultyResponseDTO>>> GetAll()
    {
        return await _httpService.Get<ResponseWrapper<List<FacultyResponseDTO>>>("Faculty/GetList");
    }

    public async Task<ResponseWrapper<List<FacultyResponseDTO>>> GetListByUniversityId(long uniId)
    {
        return await _httpService.Get<ResponseWrapper<List<FacultyResponseDTO>>>($"Faculty/GetListByUniversityId/{uniId}");
    }
}