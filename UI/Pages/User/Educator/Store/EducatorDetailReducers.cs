using Fluxor;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace UI.Pages.User.Educator.Tabs
{
    public static class EducatorReducers
    {
        [ReducerMethod]
        public static EducatorDetailState OnSet(EducatorDetailState state, EducatorDetailSetAction action)
        {
            return state with
            {
                Educator = action.Educator,
                EducatorLoading = false,
                EducatorLoaded = true
            };
        }
        [ReducerMethod(typeof(EducatorDetailLoadAction))]
        public static EducatorDetailState OnLoad(EducatorDetailState state)
        {
            return state with
            {
                EducatorLoading = true,
                EducatorLoaded = false
            };
        }
        [ReducerMethod(typeof(EducatorClearStateAction))]
        public static EducatorDetailState OnClearSet(EducatorDetailState state)
        {
            return state with
            {
                EducatorLoading = false,
                EducatorLoaded = false,
                Educator = new EducatorResponseDTO(),
            };
        }
    }
}
