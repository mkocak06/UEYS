using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.FilterModels.Base;
using Shared.ResponseModels;
using Shared.Types;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using UI.Helper;
using UI.Pages.Archive.Students.StudentDetail.Store;
using UI.Services;

namespace UI.Pages.Archive.Students.StudentDetail.Tabs
{
    public partial class EReport
    {
        [Inject] public IState<ArchiveStudentDetailState> StudentState { get; set; }
        [Inject] public IStudentPerfectionService StudentPerfectionService { get; set; }
        [Inject] public IPerformanceRatingService PerformanceRatingService { get; set; }
        [Inject] public IStudentRotationService StudentRotationService { get; set; }
        [Inject] public IStudentRotationPerfectionService StudentRotationPerfectionService { get; set; }
        [Inject] public IRotationService RotationService { get; set; }
        [Inject] public IEducationTrackingService EducationTrackingService { get; set; }
        [Inject] public IOpinionService OpinionService { get; set; }
        [Inject] public IThesisService ThesisService { get; set; }
        [Inject] public IScientificStudyService ScientificStudyService { get; set; }
        [Inject] public IExitExamService ExitExamService { get; set; }

        private List<StudentPerfectionResponseDTO> _clinicalPerfections = new();
        private List<StudentPerfectionResponseDTO> _interventionalPerfections = new();
        private PerformanceRatingResponseDTO _otherPerfections = new();
        private List<StudentRotationResponseDTO> _studentRotations = new();
        private List<StudentRotationPerfectionResponseDTO> _studentRotationPerfection = new();
        private List<EducationTrackingResponseDTO> _educationTrackings = new();
        private List<OpinionFormResponseDTO> _opinionForms = new();
        private List<ThesisResponseDTO> _theses = new();
        private List<ScientificStudyResponseDTO> _scientificStudies = new();
        private List<ExitExamResponseDTO> _exitExams = new();
        private bool _loading;
        private StudentResponseDTO Student => StudentState.Value.Student;
        private string[] perfectionValues = { "", "Bilir", "Nasıl Yapıldığını Bilir", "Gösterir", "Yapar" };
        private string[] opinionFormValues = { "", "Olumsuz", "Olumsuz", "Olumsuz", "İyi", "İyi", "İyi", "Mükemmel", "Mükemmel", "Mükemmel" };

        protected override async void OnInitialized()
        {
            _loading = true;
            StateHasChanged();
            try
            {
                var clinicals = await StudentPerfectionService.GetForEReport(Student.Id, PerfectionType.Clinical);
                if (clinicals.Item != null)
                    _clinicalPerfections = clinicals.Item;

                var interventionals = await StudentPerfectionService.GetForEReport(Student.Id, PerfectionType.Interventional);
                if (interventionals.Item != null)
                    _interventionalPerfections = interventionals.Item;

                var others = await PerformanceRatingService.GetForEReport(Student.Id);
                if (others.Item != null)
                    _otherPerfections = others.Item.FirstOrDefault();

                var studentRotations = await StudentRotationService.GetForEReport((long)Student.Id);
                if (studentRotations.Item != null)
                    _studentRotations = studentRotations.Item;

                var studentFormerRotations = await StudentRotationService.GetFormerForEReport((long)Student.UserId);
                if (studentFormerRotations.Item != null)
                    studentFormerRotations.Item.ForEach(x => _studentRotations.Add(x));

                _studentRotations.OrderBy(x => x.BeginDate).ToList();

                var educationTrackings = await EducationTrackingService.GetForEReport((long)Student.Id);
                if (educationTrackings.Item != null)
                    _educationTrackings = educationTrackings.Item.OrderBy(x => x.ProcessDate).ThenBy(x => x.ProcessType).ToList();

                var opinionForms = await OpinionService.GetForEReport((long)Student.Id);
                if (opinionForms.Item != null)
                    _opinionForms = opinionForms.Item.OrderBy(x => x.StartDate).ToList();

                var theses = await ThesisService.GetForEReport((long)Student.Id);
                if (theses.Item != null)
                    _theses = theses.Item;
                    

                var scientificStudies = await ScientificStudyService.GetForEReport((long)Student.Id);
                if (scientificStudies.Item != null)
                    _scientificStudies = scientificStudies.Item;

                var exitExams = await ExitExamService.GetForEReport(new FilterDTO { Filter = new() { Filters = new() { new() { Field = "StudentId", Operator = "eq", Value = Student.Id }, new() { Field = "IsDeleted", Operator = "eq", Value = false } }, Logic = "and" } });
                if (exitExams.Items != null)
                    _exitExams = exitExams.Items;


                _studentRotations.PrintJson("studentRotations");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }

            base.OnInitialized();
        }
    }
}
