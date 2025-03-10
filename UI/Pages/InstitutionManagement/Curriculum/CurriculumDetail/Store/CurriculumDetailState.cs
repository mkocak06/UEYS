using System;
using System.Collections.Generic;
using Fluxor;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace UI.Pages.InstitutionManagement.Curriculum.CurriculumDetail.Store
{
    public record CurriculumDetailState
    {
        public bool CurriculumsLoading { get; init; }
        public bool CurriculumsLoaded { get; init; }
        public bool RotationsLoading { get; init; }
        public bool RotationsLoaded { get; init; }
        public bool PerfectionsLoading { get; init; }
        public bool PerfectionsLoaded { get; init; }

        public CurriculumResponseDTO Curriculum { get; init; }

        public List<RotationResponseDTO> Rotations { get; init; }
        public List<PerfectionResponseDTO> Perfections { get; init; }

    }

    public class CurriculumDetailFeature : Feature<CurriculumDetailState>
    {
        public override string GetName() => "CurriculumDetail";
        protected override CurriculumDetailState GetInitialState()
        {
            return new CurriculumDetailState
            {
                CurriculumsLoaded = false,
                CurriculumsLoading = false,
                RotationsLoaded = false,
                RotationsLoading = false,
                PerfectionsLoaded = false,
                PerfectionsLoading = false,
                Curriculum = null,
                Rotations = new(),
                Perfections = new()
            };
        }
    }

    #region CurriculumDetail Action

    public record CurriculumDetailLoadAction(long Id);
    public record CurriculumDetailSetAction(CurriculumResponseDTO Curriculum);
    public record CurriculumDetailUpdateAction(CurriculumResponseDTO Curriculum);
    public record CurriculumDetailUpdateSuccessAction(CurriculumResponseDTO Curriculum);
    public record CurriculumDetailDeleteAction(CurriculumResponseDTO Curriculum);
    public record CurriculumDetailDeleteSuccessAction();

    public record CurriculumClearStateAction();

    #endregion
}
