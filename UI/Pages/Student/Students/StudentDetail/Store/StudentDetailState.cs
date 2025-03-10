using System;
using System.Collections.Generic;
using Fluxor;
using Shared.RequestModels;
using Shared.ResponseModels;

namespace UI.Pages.Student.Students.StudentDetail.Store
{
    public record StudentDetailState
    {
        public bool StudentLoading { get; init; }
        public bool StudentLoaded { get; init; }
        public bool RotationsLoading { get; init; }
        public bool RotationsLoaded { get; init; }
        public bool PerfectionsLoading { get; init; }
        public bool PerfectionsLoaded { get; init; }
        public bool PerformanceRatingLoading { get; init; }
        public bool PerformanceRatingLoaded { get; init; }

        public StudentResponseDTO Student { get; init; }
        public StudentResponseDTO StudentAdd { get; init; }

        public List<RotationResponseDTO> Rotations { get; init; }
        public List<PerfectionResponseDTO> Perfections { get; init; }
        public List<PerformanceRatingResponseDTO> Ratings { get; init; }

    }

    public class StudentDetailFeature : Feature<StudentDetailState>
    {
        public override string GetName() => "StudentDetail";
        protected override StudentDetailState GetInitialState()
        {
            return new StudentDetailState
            {
                StudentLoaded = false,
                StudentLoading = false,
                RotationsLoaded = false,
                RotationsLoading = false,
                PerfectionsLoaded = false,
                PerfectionsLoading = false,
                PerformanceRatingLoading = false,
                PerformanceRatingLoaded=false,
                Student = null,
                StudentAdd = new StudentResponseDTO(),
                Rotations = new(),
                Perfections = new(),
                Ratings=new()
            };
        }
    }

    #region StudentDetail Action

    public record StudentDetailLoadAction(long Id);
    public record StudentDetailSetAction(StudentResponseDTO Student);
    public record StudentDetailUpdateAction(StudentResponseDTO Student);
    public record StudentDetailUpdateSuccessAction(StudentResponseDTO Student);
    public record StudentDetailDeleteAction(StudentResponseDTO Curriculum);
    public record StudentDetailDeleteSuccessAction();
                
    public record StudentDetailAddNewAction(StudentDTO Student);
    public record StudentDetailAddSuccessAction(StudentDTO Student);
    public record StudentDetailAddFailureAction(StudentDTO Student);



    public record StudentClearStateAction();

    #endregion
}
