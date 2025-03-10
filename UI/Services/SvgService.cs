using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;

namespace UI.Services;

public interface ISvgService
{
    Task<ResponseWrapper<SvgResponseDTO>> GetById(string id);
}

public class SvgService : ISvgService
{
    private readonly IHttpService _httpService;

    public SvgService(IHttpService httpService)
    {
        _httpService = httpService;
    }
    public async Task<ResponseWrapper<SvgResponseDTO>> GetById(string id)
    {
        return await _httpService.Get<ResponseWrapper<SvgResponseDTO>>($"Svg/GetById?id={id}");
    }

}