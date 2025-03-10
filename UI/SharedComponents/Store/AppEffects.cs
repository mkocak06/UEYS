using AutoMapper;
using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Services;

namespace UI.SharedComponents.Store;

public class AppEffects
{
    private readonly IMapper _mapper;
    private readonly IState<AppState> _state;
    private readonly IProvinceService _provinceService;
    private readonly IMenuService _menuService;
    private readonly IExpertiseBranchService _expertiseBranchService;
    
    public AppEffects(IMapper mapper, IState<AppState> state, IProvinceService provinceService, IMenuService menuService, IExpertiseBranchService expertiseBranchService )
    {
        _mapper = mapper;
        _state = state;
        this._provinceService = provinceService;
        this._menuService = menuService;
        this._expertiseBranchService = expertiseBranchService;
    }

    [EffectMethod]
    public async Task LoadMenus(AppLoadMenusAction action, IDispatcher dispatcher)
    {
        try
        {
            var response = await _menuService.GetAll();
            if(response.Result)
            {
                dispatcher.Dispatch(new AppSetMenuAction(response.Item.OrderBy(x => x.Order).ToList()));
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    [EffectMethod]
    public async Task LoadProvinces(AppLoadProvincesAction action, IDispatcher dispatcher)
    {
        try
        {
            var response = await _provinceService.GetAll();
            if (response.Result)
            {
                dispatcher.Dispatch(new AppSetProvincesAction(response.Item.OrderBy(x=>x.Name).ToList()));
            }
            else
            {
                dispatcher.Dispatch(new AppFailureProvincesAction(response));
            }
        }
        catch (Exception e)
        {
            dispatcher.Dispatch(new AppFailureProvincesAction(new ResponseWrapper<List<ProvinceResponseDTO>>() { Result = false, Message = e.Message }));
        }
    }

}
