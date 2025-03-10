using Fluxor;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System.Collections.Generic;

namespace UI.Pages.Hospitals.HospitalDetail.Store
{
    public record HospitalDetailState
    {
        public bool HospitalDetailLoading { get; init; }
        public bool HospitalDetailLoaded { get; init; }
        public bool AddHospitalDetailLoading { get; init; }
        public bool AddHospitalDetailLoaded { get; init; }
        public bool UpdateHospitalDetailLoading { get; init; }
        public bool UpdateHospitalDetailLoaded { get; init; }
        public HospitalResponseDTO Hospital { get; init; }
    }

    public class HospitalDetailFeature : Feature<HospitalDetailState>
    {
        public override string GetName() => "HospitalDetail";

        protected override HospitalDetailState GetInitialState()
        {
            return new HospitalDetailState
            {
                HospitalDetailLoaded = false,
                HospitalDetailLoading = false,
                UpdateHospitalDetailLoading = false,
                UpdateHospitalDetailLoaded = false,
                Hospital= null,
            };
        }
    }

    #region UserActions
    public record HospitalDetailLoadAction(long id);
    public record HospitalDetailSetAction(HospitalResponseDTO hospitalDetail);
    public record HospitalDetailFailureAction(ResponseWrapper<HospitalResponseDTO> error);
    public record HospitalDetailClearStateAction();

    public record HospitalDetailUpdateAction(long id, HospitalDTO hospitalDetail);
    public record HospitalDetailUpdateSuccessAction(HospitalResponseDTO hospitalDetail);
    public record HospitalDetailUpdateFailureAction(ResponseWrapper<HospitalResponseDTO> error);

    #endregion
}