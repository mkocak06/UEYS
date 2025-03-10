using System;
namespace Shared.ResponseModels.StatisticModels
{
	public class CityDetailsForMapModel
	{
		public string ProvinceName { get; set; }
		public int UniversityCount { get; set; }
		public int HospitalCount { get; set; }
		public int FacultyCount { get; set; }
        public int ProgramCount { get; set; }
		public int EducatorCount { get; set; }
		public int StudentCount { get; set; }
	}
}

