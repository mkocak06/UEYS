using System.Collections.Generic;

namespace Core.Entities
{
    public class SubQuotaRequest : ExtendedBaseEntity
    {
        public long? ProgramId { get; set; }
        public Program Program { get; set; }
        public long? QuotaRequestId { get; set; }
        public QuotaRequest QuotaRequest { get; set; }

        public int? EducatorIndex { get; set; }
        public int? SpecialistDoctorCount { get; set; }
        public int? ProfessorCount { get; set; }
        public int? AssociateProfessorCount { get; set; }
        public int? DoctorLecturerCount { get; set; }
        public int? ChiefAssistantCount { get; set; }

        public int? TotalEducatorCount { get; set; }
        public int? CurrentStudentCount { get; set; }
        public int? StudentWhoLast6MonthToFinishCount { get; set; }

        public int? PortfolioIndex { get; set; }
        public int? CapacityIndex { get; set; }
        public int? Capacity { get; set; }

        public virtual ICollection<SubQuotaRequestPortfolio> SubQuotaRequestPortfolios { get; set; }
        public virtual ICollection<StudentCount> StudentCounts { get; set; }

    }
}
