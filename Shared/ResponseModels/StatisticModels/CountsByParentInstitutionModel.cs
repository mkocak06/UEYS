using System;
namespace Shared.ResponseModels.StatisticModels
{
	public class CountsByParentInstitutionModel
    {
		public string ParentInstitutionName { get; set; }
		public long UniversityCount { get; set; }
		public long HospitalCount { get; set; }
        public long ProgramCount { get; set; }
		public long EducatorCount { get; set; }
		public long StudentCount { get; set; }
	}
}

