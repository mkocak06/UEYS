using System;
using System.Globalization;

namespace Shared.ResponseModels.Program
{
	public class ProgramChartModel
	{
        public long Id { get; set; }

        public string ProvinceName { get; set; }
        public string ProvincePlateCode { get; set; }
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
		public string AuthorizationCategoryColorCode { get; set; }
		public long? AuthorizationCategoryId { get; set; }
		public string ProfessionName { get; set; }
		public int? ProfessionCode{ get; set; }
        public long? ProfessionId { get; set; }
        public string ParentInstitutionName { get; set; }
        public long? ParentInstitutionId { get; set; }
        public bool? IsPrincipal { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

