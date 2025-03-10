using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Pages.User.Educator.Tabs;
using UI.Services;

namespace UI.Pages.User.Educator.Store
{
    public class EducatorDetailEffects
    {
        private readonly IEducatorService EducatorService;

        public EducatorDetailEffects(IEducatorService EducatorService)
        {
            this.EducatorService = EducatorService;
        }

        [EffectMethod]
        public async Task LoadEducators(EducatorDetailLoadAction action, IDispatcher dispatcher)
        {
            ResponseWrapper<EducatorResponseDTO> currentEducator;
            try
            {
                currentEducator = await EducatorService.GetById(action.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                currentEducator = new();
            }
            dispatcher.Dispatch(new EducatorDetailSetAction(currentEducator.Item));
        }
    }
}
