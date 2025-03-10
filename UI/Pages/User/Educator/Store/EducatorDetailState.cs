using System;
using System.Collections.Generic;
using Fluxor;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace UI.Pages.User.Educator.Tabs
{
    public record EducatorDetailState
    {
        public bool EducatorLoading { get; init; }
        public bool EducatorLoaded { get; init; }
        public bool RotationsLoading { get; init; }
        public bool RotationsLoaded { get; init; }
        public bool PerfectionsLoading { get; init; }
        public bool PerfectionsLoaded { get; init; }

        public EducatorResponseDTO Educator { get; init; }
        public EducatorResponseDTO EducatorAdd { get; init; }

        public List<RotationResponseDTO> Rotations { get; init; }
        public List<PerfectionResponseDTO> Perfections { get; init; }

    }

    public class EducatorDetailFeature : Feature<EducatorDetailState>
    {
        public override string GetName() => "EdacatorDetail";
        protected override EducatorDetailState GetInitialState()
        {
            return new EducatorDetailState
            {
                EducatorLoaded = false,
                EducatorLoading = false,
                RotationsLoaded = false,
                RotationsLoading = false,
                PerfectionsLoaded = false,
                PerfectionsLoading = false,
                Educator = null,
                EducatorAdd = new EducatorResponseDTO(),
                Rotations = new(),
                Perfections = new()
            };
        }
    }

    #region EducatorDetail Action

    public record EducatorDetailLoadAction(long Id);
    public record EducatorDetailSetAction(EducatorResponseDTO Educator);
    public record EducatorDetailUpdateAction(EducatorResponseDTO Educator);
    public record EducatorDetailUpdateSuccessAction(EducatorResponseDTO Educator);
    public record EducatorDetailDeleteAction(EducatorResponseDTO Curriculum);
    public record EducatorDetailDeleteSuccessAction();

    public record EducatorDetailAddNewAction(EducatorDTO Educator);
    public record EducatorDetailAddSuccessAction(EducatorDTO Educator);
    public record EducatorDetailAddFailureAction(EducatorDTO Educator);



    public record EducatorClearStateAction();

    #endregion
}
