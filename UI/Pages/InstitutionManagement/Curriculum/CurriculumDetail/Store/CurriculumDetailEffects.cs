using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store
{
    public class CurriculumDetailEffects
    {
        private readonly ICurriculumService curriculumService;

        public CurriculumDetailEffects(ICurriculumService curriculumService)
        {
            this.curriculumService = curriculumService;
        }

        [EffectMethod]
        public async Task LoadCurriculums(CurriculumDetailLoadAction action, IDispatcher dispatcher)
        {
            ResponseWrapper<CurriculumResponseDTO> currentCurricula;
            try
            {

                currentCurricula = await curriculumService.GetById(action.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                currentCurricula = new();
            }
            dispatcher.Dispatch(new CurriculumDetailSetAction(currentCurricula.Item));
        }
    }
}
