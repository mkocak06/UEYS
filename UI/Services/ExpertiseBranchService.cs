using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IExpertiseBranchService
{
    Task<PaginationModel<ExpertiseBranchResponseDTO>> GetPaginateList(FilterDTO filter);
    Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListRelatedWithProgramsByProfessionId(long id);
    Task<ResponseWrapper<ExpertiseBranchResponseDTO>> GetById(long id);
    Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListByProfessionId(long id);
    Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListForProtocolProgramByHospitalId(long hospitalId);
    Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Add(ExpertiseBranchDTO expertiseBranch);
    Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Update(long id, ExpertiseBranchDTO expertiseBranch);
    Task Delete(long id);
}

public class ExpertiseBranchService : IExpertiseBranchService
{
    private readonly IHttpService _httpService;

    public ExpertiseBranchService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<PaginationModel<ExpertiseBranchResponseDTO>> GetPaginateList(FilterDTO filter)
    {
        return await _httpService.Post<PaginationModel<ExpertiseBranchResponseDTO>>($"ExpertiseBranch/GetPaginateList", filter);
    }
    
    public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListRelatedWithProgramsByProfessionId(long id)
    {
        return await _httpService.Get<ResponseWrapper<List<ExpertiseBranchResponseDTO>>>($"ExpertiseBranch/GetListRelatedWithProgramsByProfessionId/{id}");
    }

    public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListForProtocolProgramByHospitalId(long hospitalId)
    {
        return await _httpService.Get<ResponseWrapper<List<ExpertiseBranchResponseDTO>>>($"ExpertiseBranch/GetListForProtocolProgramByHospitalId/{hospitalId}");
    }

    public async Task<ResponseWrapper<ExpertiseBranchResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<ExpertiseBranchResponseDTO>>($"ExpertiseBranch/Get/{id}");
    }
    public async Task<ResponseWrapper<List<ExpertiseBranchResponseDTO>>> GetListByProfessionId(long id)
    {
        return await _httpService.Get<ResponseWrapper<List<ExpertiseBranchResponseDTO>>>($"ExpertiseBranch/GetListByProfessionId/{id}");
    }

    public async Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Add(ExpertiseBranchDTO expertiseBranch)
    {
        return await _httpService.Post<ResponseWrapper<ExpertiseBranchResponseDTO>>($"ExpertiseBranch/Post", expertiseBranch);
    }

    public async Task<ResponseWrapper<ExpertiseBranchResponseDTO>> Update(long id, ExpertiseBranchDTO expertiseBranch)
    {
        return await _httpService.Put<ResponseWrapper<ExpertiseBranchResponseDTO>>($"ExpertiseBranch/Put/{id}", expertiseBranch);
    }
    
    public async Task Delete(long id)
    {
        await _httpService.Delete($"ExpertiseBranch/Delete/{id}");
    }
}