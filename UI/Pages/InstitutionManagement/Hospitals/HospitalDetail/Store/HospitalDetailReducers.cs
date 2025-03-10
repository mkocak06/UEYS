using System;
using System.Collections.Generic;
using System.Linq;
using Fluxor;
using UI.Helper;

namespace UI.Pages.Hospitals.HospitalDetail.Store
{
    public static class HospitalDetailReducers
    {
        [ReducerMethod(typeof(HospitalDetailLoadAction))]
        public static HospitalDetailState OnLoad(HospitalDetailState state)
        {
            return state with
            {
                HospitalDetailLoading = true,
                HospitalDetailLoaded = false
            };
        }
        [ReducerMethod]
        public static HospitalDetailState OnSet(HospitalDetailState state, HospitalDetailSetAction action)
        {
            return state with
            {
                HospitalDetailLoading = false,
                HospitalDetailLoaded = true,
                Hospital= action.hospitalDetail
            };
        }
        [ReducerMethod]
        public static HospitalDetailState OnFailure(HospitalDetailState state, HospitalDetailFailureAction action)
        {
            return state with
            {
                HospitalDetailLoading = false,
                HospitalDetailLoaded = true,
            };
        }

        [ReducerMethod]
        public static HospitalDetailState OnUpdateLoad(HospitalDetailState state, HospitalDetailUpdateAction action)
        {
            return state with
            {
                UpdateHospitalDetailLoading = true,
                UpdateHospitalDetailLoaded = false
            };
        }
        [ReducerMethod]
        public static HospitalDetailState OnUpdateSet(HospitalDetailState state, HospitalDetailUpdateSuccessAction action)
        {
            return state with
            {
                UpdateHospitalDetailLoading = false,
                UpdateHospitalDetailLoaded = true,
                Hospital= action.hospitalDetail
            };
        }
        [ReducerMethod]
        public static HospitalDetailState OnUpdateFailure(HospitalDetailState state, HospitalDetailUpdateFailureAction action)
        {
            return state with
            {
                UpdateHospitalDetailLoading = false,
                UpdateHospitalDetailLoaded = true,
            };
        }
        
        [ReducerMethod]
        public static HospitalDetailState OnClearState(HospitalDetailState state, HospitalDetailClearStateAction action)
        {
            return state with
            {
                HospitalDetailLoaded = false,
                HospitalDetailLoading = false,
                UpdateHospitalDetailLoading = false,
                UpdateHospitalDetailLoaded = false,
                Hospital = null,
            };
        }

    }
}