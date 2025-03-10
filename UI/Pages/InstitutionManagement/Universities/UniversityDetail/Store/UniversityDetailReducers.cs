using Fluxor;
using Shared.ResponseModels;
using System.Collections.Generic;

namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store
{
    public static class UniversityDetailReducers
    {

        [ReducerMethod]
        public static UniversityDetailState OnSet(UniversityDetailState state, UniversityDetailSetAction action)
        {
            return state with
            {
                University = action.University,
                UniversitiesLoading = false,
                UniversitiesLoaded = true
            };
        }

        [ReducerMethod]
        public static UniversityDetailState OnSetAffiliation(UniversityDetailState state, AffiliationsSetAction action)
        {
            return state with
            {
                Affiliations = action.Affiliations,
                AffiliationsLoading = false,
                AffiliationsLoaded = true
            };
        }

        [ReducerMethod]
        public static UniversityDetailState OnSetHospital(UniversityDetailState state, HospitalsSetAction action)
        {
            return state with
            {
                Hospitals = action.Hospitals,
                HospitalsLoading = false,
                HospitalsLoaded = true
            };
        }

        [ReducerMethod(typeof(UniversityClearStateAction))]
        public static UniversityDetailState OnClearSet(UniversityDetailState state)
        {
            return state with
            {
                UniversitiesLoading = false,
                UniversitiesLoaded = false,
                AffiliationsLoading = false,
                AffiliationsLoaded = false,
                University = new UniversityResponseDTO(),
                Affiliations = new List<AffiliationResponseDTO>()
            };
        }

        [ReducerMethod(typeof(UniversityDetailLoadAction))]
        public static UniversityDetailState OnLoad(UniversityDetailState state)
        {
            return state with
            {
                UniversitiesLoading = true,
                UniversitiesLoaded = false
            };
        }
        [ReducerMethod(typeof(AffiliationsLoadAction))]
        public static UniversityDetailState OnAffiliationsLoad(UniversityDetailState state)
        {
            return state with
            {
                AffiliationsLoading = true,
                AffiliationsLoaded = false
            };
        }



    }
}
