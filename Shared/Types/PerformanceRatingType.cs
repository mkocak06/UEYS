using System.ComponentModel;

namespace Shared.Types
{
    public enum PerformanceRatingType
    {
        [Description("5.1.1.1 Multiple Choice Test")]
        MultipleChoiceTest = 1,
        [Description("5.1.1.2 Short Answer Written Test")]
        ShortAnswerWrittenTest = 2,
        [Description("5.1.1.3 Oral Exam")]
        OralExam = 3,
        [Description("5.1.2.1 Long Answer Written Test")]
        LongAnswerWrittenTest = 4,
        [Description("5.1.2.2 Extended Matching Questions")]
        ExtendedMatchingQuestions = 5,
        [Description("5.1.2.3 Key Featured Questions")]
        KeyFeaturedQuestions = 6,
        [Description("5.2.1.1 Standard Patient Interview")]
        StandardPatientInterview = 7,
        [Description("5.2.1.2 Simulation Evaluations")]
        SimulationEvaluations = 8,
        [Description("5.2.1.3 Structured Objective Clinical Exams")]
        StructuredObjectiveClinicalExams = 9,
        [Description("5.3.1.1 Mini Clinical Evaluation Test")]
        MiniClinicalEvaluationTest = 10, 
        [Description("5.3.1.2 Direct Observation Of Skills")]
        DirectObservationOfSkills = 9,
        [Description("5.3.1.3 Ration Card")]
        RationCard = 10,
        [Description("5.3.1.4 Improvement File")]
        ImprovementFile = 10,
        [Description("5.3.1.5 360 Degrees Evaluation")]
        Evaluation360Degrees = 9,
        [Description("5.4.1.1 360 Degrees Evaluation")]
        Evaluation360Degrees_1 = 10,
        [Description("5.4.1.2 Graduate Monitoring")]
        GraduateMonitoring= 10
    }
}
