using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IRelatedDependentProgramService
{
    Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Add(RelatedDependentProgramDTO relatedDependentProgram);
    Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Update(long id, RelatedDependentProgramDTO relatedDependentProgram);

    Task Delete(long id);
}

public class RelatedDependentProgramService : IRelatedDependentProgramService
{
    private readonly IHttpService _httpService;

    public RelatedDependentProgramService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Add(RelatedDependentProgramDTO relatedDependentProgram)
    {
        return await _httpService.Post<ResponseWrapper<RelatedDependentProgramResponseDTO>>($"RelatedDependentProgram/Post", relatedDependentProgram);
    }

    public async Task<ResponseWrapper<RelatedDependentProgramResponseDTO>> Update(long id, RelatedDependentProgramDTO relatedDependentProgram)
    {
        return await _httpService.Put<ResponseWrapper<RelatedDependentProgramResponseDTO>>($"RelatedDependentProgram/Put/{id}", relatedDependentProgram);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"RelatedDependentProgram/Delete/{id}");
    }
}