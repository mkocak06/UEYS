using Fluxor;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store
{
    public static class CurriculumDetailReducers
    {
        [ReducerMethod]
        public static CurriculumDetailState OnSet(CurriculumDetailState state, CurriculumDetailSetAction action)
        {
            return state with
            {
                Curriculum = action.Curriculum,
                CurriculumsLoading = false,
                CurriculumsLoaded = true
            };
        }
        [ReducerMethod(typeof(CurriculumDetailLoadAction))]
        public static CurriculumDetailState OnLoad(CurriculumDetailState state)
        {
            return state with
            {
                CurriculumsLoading = true,
                CurriculumsLoaded = false
            };
        }
        [ReducerMethod(typeof(CurriculumClearStateAction))]
        public static CurriculumDetailState OnClearSet(CurriculumDetailState state)
        {
            return state with
            {
                CurriculumsLoading = false,
                CurriculumsLoaded = false,
                Curriculum = new CurriculumResponseDTO(),
            };
        }
    }
}
