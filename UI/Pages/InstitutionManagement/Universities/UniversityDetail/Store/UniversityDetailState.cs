using System;
using System.Collections.Generic;
using Fluxor;
using Shared.ResponseModels;

namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store
{
    public record UniversityDetailState
    {
        public bool UniversitiesLoading { get; init; }
        public bool UniversitiesLoaded { get; init; }
        public bool AffiliationsLoading { get; init; }
        public bool AffiliationsLoaded { get; init; }
        public bool HospitalsLoading { get; init; }
        public bool HospitalsLoaded { get; init; }

        public UniversityResponseDTO University { get; init; }

        public List<AffiliationResponseDTO> Affiliations { get; init; }
        public List<HospitalResponseDTO> Hospitals { get; init; }

    }

    public class UniversityDetailFeature : Feature<UniversityDetailState>
    {
        public override string GetName() => "UniversityDetail";
        protected override UniversityDetailState GetInitialState()
        {
            return new UniversityDetailState
            {
                UniversitiesLoaded = false,
                UniversitiesLoading = false,
                AffiliationsLoaded=false,
                AffiliationsLoading=false,
                HospitalsLoaded=false,
                HospitalsLoading=false,
                University = null,
                Affiliations=new(),
                Hospitals=new()
            };
        }
    }

    #region UniversityDetail Action

    public record UniversityDetailLoadAction(long Id);
    public record UniversityDetailSetAction(UniversityResponseDTO University);
    public record UniversityDetailUpdateAction(UniversityResponseDTO University);
    public record UniversityDetailUpdateSuccessAction(UniversityResponseDTO University);
    public record UniversityDetailDeleteAction(UniversityResponseDTO University);
    public record UniversityDetailDeleteSuccessAction();

    public record AffiliationsLoadAction(long Id);
    public record AffiliationsSetAction(List<AffiliationResponseDTO> Affiliations);
    public record AffiliationUpdateAction(AffiliationResponseDTO Affiliation);
    public record AffiliationUpdateSuccessAction(AffiliationResponseDTO Affiliation);
    public record AffiliationDeleteAction(AffiliationResponseDTO Affiliation);
    public record AffiliationDeleteSuccessAction();

    public record HospitalsLoadAction(long Id);
    public record HospitalsSetAction(List<HospitalResponseDTO> Hospitals);
    public record HospitalUpdateAction(HospitalResponseDTO Hospital);



    public record UniversityClearStateAction();

    #endregion
}
