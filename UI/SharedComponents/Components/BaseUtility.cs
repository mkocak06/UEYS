using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UI.SharedComponents.Components;

public class BaseUtility : FluxorComponent
{
    [Inject] private IJSRuntime JsRuntime { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }
    
    [Parameter] public string BaseClass { get; set; }
    [Parameter] public string ChildClass { get; set; }
    [Parameter] public bool InitJS { get; set; }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && InitJS)
            JsRuntime.InvokeAsync<bool>("createBootstrapInstances");
        return base.OnAfterRenderAsync(firstRender);
    }
}