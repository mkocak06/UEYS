using System.Threading.Tasks;
using Microsoft.JSInterop;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Web;
using System;
using System.Text.RegularExpressions;

namespace UI.Services;


public interface IIconHelperService
{
    Task<string> GetIcon(string name);
}

public class IconHelperService : IIconHelperService
{
    private IJSRuntime Js { get; set; }
    private readonly ISvgService _svgService;

    public IconHelperService(IJSRuntime js, ISvgService svgService)
    {
        Js = js;
        _svgService = svgService;
    }
    
    public async Task<string> GetIcon(string name)
    {
        //return await Js.InvokeAsync<string>("getSvgIcon", name);
        var svgObject = await _svgService.GetById(name);
        if (svgObject.Result)
        {
            return Regex.Unescape(svgObject.Item.Content);
        }

        return Regex.Unescape("<!--begin::Svg Icon | path:/var/www/preview.keenthemes.com/keen/releases/2021-04-21-040700/theme/demo1/dist/../src/media/svg/icons/Devices/Display1.svg-->\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"24px\" height=\"24px\" viewBox=\"0 0 24 24\" version=\"1.1\">\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<g stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\">\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<rect x=\"0\" y=\"0\" width=\"24\" height=\"24\"></rect>\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<path d=\"M11,20 L11,17 C11,16.4477153 11.4477153,16 12,16 C12.5522847,16 13,16.4477153 13,17 L13,20 L15.5,20 C15.7761424,20 16,20.2238576 16,20.5 C16,20.7761424 15.7761424,21 15.5,21 L8.5,21 C8.22385763,21 8,20.7761424 8,20.5 C8,20.2238576 8.22385763,20 8.5,20 L11,20 Z\" fill=\"#000000\" opacity=\"0.3\"></path>\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<path d=\"M3,5 L21,5 C21.5522847,5 22,5.44771525 22,6 L22,16 C22,16.5522847 21.5522847,17 21,17 L3,17 C2.44771525,17 2,16.5522847 2,16 L2,6 C2,5.44771525 2.44771525,5 3,5 Z M4.5,8 C4.22385763,8 4,8.22385763 4,8.5 C4,8.77614237 4.22385763,9 4.5,9 L13.5,9 C13.7761424,9 14,8.77614237 14,8.5 C14,8.22385763 13.7761424,8 13.5,8 L4.5,8 Z M4.5,10 C4.22385763,10 4,10.2238576 4,10.5 C4,10.7761424 4.22385763,11 4.5,11 L7.5,11 C7.77614237,11 8,10.7761424 8,10.5 C8,10.2238576 7.77614237,10 7.5,10 L4.5,10 Z\" fill=\"#000000\"></path>\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</g>\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</svg>\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<!--end::Svg Icon-->");
    }
}