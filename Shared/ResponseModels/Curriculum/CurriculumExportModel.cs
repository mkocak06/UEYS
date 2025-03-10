using System;
using System.Collections.Generic;

namespace Shared.ResponseModels.Curriculum
{
	public class CurriculumExportModel
	{
        public CurriculumExportModel()
        {
            Rotations = new();
            Perfections = new();
        }
		public string CurriculumName { get; set; }
		public string CurriculumVersion { get; set; }
		public string CurriculumDuration { get; set; }
		public string EffectiveDate { get; set; }
		public string ProfessionName { get; set; }
		public string IsActive { get; set; }
        public List<RotationModel> Rotations { get; set; }
        public List<PerfectionModel> Perfections { get; set; }
    }
    public class RotationModel
    {
        public RotationModel()
        {
            Perfections = new();
        }
        public string RotationName { get; set; }
        public string RotationDuration { get; set; }
        public string IsRequired { get; set; }
        public List<PerfectionModel> Perfections { get; set; }
    }
    public class PerfectionModel
    {
        public string PerfectionType { get; set; }
        public string GroupName { get; set; }
        public string PerfectionName { get; set; }
        public string SeniorityName { get; set; }
        public string Levels { get; set; }
        public string Methods { get; set; }
    }
}
