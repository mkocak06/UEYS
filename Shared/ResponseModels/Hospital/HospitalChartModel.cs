using System;
using System.Diagnostics.Metrics;
using Shared.Types;

namespace Shared.ResponseModels.Hospital
{
	public class HospitalChartModel
	{
        public long Id { get; set; }

        public string ProvinceName { get; set; }
        public long? ProvinceId { get; set; }
        public string ExpertiseBranchName { get; set; }
        public long? ExpertiseBranchId { get; set; }
        public string ProgramName { get; set; }
        public string HospitalName { get; set; }
        public long? HospitalId { get; set; }
        public string UniversityName { get; set; }
        public bool? IsUniversityPrivate { get; set; }
        public long? UniversityId { get; set; }
        public string FacultyName { get; set; }
        public long? FacultyId { get; set; }
        public string AuthorizationCategory { get; set; }
        public bool? AuthorizationCategoryIsActive { get; set; }
        public long? AuthorizationCategoryId { get; set; }
        public string ProfessionName { get; set; }
        public long? ProfessionId { get; set; }
        public string ParentInstitutionName { get; set; }
        public long? ParentInstitutionId { get; set; }
        public bool? IsPrincipal { get; set; }
        public bool? IsDeleted { get; set; }

        public bool? IsForeign { get; set; }
        public long? CountryId { get; set; }
        public string CountryName { get; set; }
    }
}

