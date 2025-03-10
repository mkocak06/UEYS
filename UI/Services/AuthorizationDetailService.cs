using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface IAuthorizationDetailService
{
    Task<ResponseWrapper<AuthorizationDetailResponseDTO>> GetById(long id);
    Task<ResponseWrapper<List<AuthorizationDetailResponseDTO>>> GetListByProgramId(long programId);
    Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Add(AuthorizationDetailDTO authorizationDetail);
    Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Update(long id, AuthorizationDetailDTO authorizationDetail);
    Task Delete(long id);
}

public class AuthorizationDetailService : IAuthorizationDetailService
{
    private readonly IHttpService _httpService;

    public AuthorizationDetailService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<ResponseWrapper<AuthorizationDetailResponseDTO>> GetById(long id)
    {
        return await _httpService.Get<ResponseWrapper<AuthorizationDetailResponseDTO>>($"AuthorizationDetail/Get/{id}");
    }

    public async Task<ResponseWrapper<List<AuthorizationDetailResponseDTO>>> GetListByProgramId(long programId)
    {
        return await _httpService.Get<ResponseWrapper<List<AuthorizationDetailResponseDTO>>>($"AuthorizationDetail/GetListByProgramId/{programId}");
    }

    public async Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Add(AuthorizationDetailDTO authorizationDetail)
    {
        return await _httpService.Post<ResponseWrapper<AuthorizationDetailResponseDTO>>($"AuthorizationDetail/Post", authorizationDetail);
    }

    public async Task<ResponseWrapper<AuthorizationDetailResponseDTO>> Update(long id, AuthorizationDetailDTO authorizationDetail)
    {
        return await _httpService.Put<ResponseWrapper<AuthorizationDetailResponseDTO>>($"AuthorizationDetail/Put/{id}", authorizationDetail);
    }

    public async Task Delete(long id)
    {
        await _httpService.Delete($"AuthorizationDetail/Delete/{id}");
    }
}