namespace Shared.BaseModels
{
    public class SubQuotaRequestBase
    {
        public int? EducatorIndex { get; set; }
        public int? PortfolioIndex { get; set; }
        public int? CapacityIndex { get; set; }
        public int? Capacity { get; set; }

        public long? ProgramId { get; set; }
        public long? QuotaRequestId { get; set; }

        public int? SpecialistDoctorCount { get; set; }
        public int? ProfessorCount { get; set; }
        public int? AssociateProfessorCount { get; set; }
        public int? DoctorLecturerCount { get; set; }
        public int? ChiefAssistantCount { get; set; }

        public int? TotalEducatorCount { get; set; }
        public int? CurrentStudentCount { get; set; }
        public int? StudentWhoLast6MonthToFinishCount { get; set; }

        public int? EducatorPoint { get; set; }

    }
}
