using Shared.BaseModels;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class SubQuotaRequestPaginateResponseDTO
    {
        public long? Id { get; set; }
        public int? EducatorIndex { get; set; }
        public int? PortfolioIndex { get; set; }
        public int? CapacityIndex { get; set; }
        public int? EducatorPoint { get; set; }

        public int? Capacity { get; set; }

        public long? ExpertiseBranchId { get; set; }
        public long? ProgramId { get; set; }
        public string ProgramName { get; set; }
        public long? QuotaRequestId { get; set; }

        public string ProvinceName { get; set; }
        public string HospitalName { get; set; }
        public string ExpertiseBranchName { get; set; }

        public int? SpecialistDoctorCount { get; set; }
        public int? ProfessorCount { get; set; }
        public int? AssociateProfessorCount { get; set; }
        public int? DoctorLecturerCount { get; set; }
        public int? ChiefAssistantCount { get; set; }

        public int? TotalEducatorCount { get; set; }
        public int? CurrentStudentCount { get; set; }
        public int? StudentWhoLast6MonthToFinishCount { get; set; }
        //public bool IsDeleted { get; set; }

        public float? StudentIndexPerEducator
        {
            get
            {
                return ((float)this.CurrentStudentCount / (float)this.TotalEducatorCount);
            }
        }
        public List<SubQuotaRequestPortfolioResponseDTO_2> Portfolios { get; set; }
        public List<StudentCountResponseDTO> StudentCounts { get; set; }

    }
}
