using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.LogInformation
{
    public class DetailedLogInformation
    {
        public string ProvinceName { get; set; }
        public string ParentInstitutionName { get; set; }
        public string UniversityName { get; set; }
        public string FacultyName { get; set; }
        public string HospitalName { get; set; }
        public int? TotalUserCount { get; set; }
        public int? TotalStudentCount { get; set; }
        public int? TotalEducatorCount { get; set; }

        public int? TodaysLoggedInStudentCount { get; set; }
        public int? TotalLoggedInStudentCount { get; set; }
        public int? TodaysCreatedStudentCount { get; set; }
        public int? TotalCreatedStudentCount { get; set; }

        public int? TodaysLoggedInEducatorCount { get; set; }
        public int? TotalLoggedInEducatorCount { get; set; }
        public int? TodaysCreatedInEducatorCount { get; set; }
        public int? TotalCreatedEducatorCount { get; set; }

        public int? TodaysLoggedInHeadCount { get; set; }
        public int? TotalLoggedInHeadCount { get; set; }
        public int? TodaysCreatedHeadCount { get; set; }
        public int? TotalCreatedHeadCount { get; set; }

        public int? TodaysLoggedInAgentCount { get; set; }
        public int? TotalLoggedInAgentCount { get; set; }
        public int? TodaysCreatedAgentCount { get; set; }
        public int? TotalCreatedAgentCount { get; set; }

    }
}