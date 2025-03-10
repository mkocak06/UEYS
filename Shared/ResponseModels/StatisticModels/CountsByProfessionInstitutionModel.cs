using System;
using System.Collections.Generic;
namespace Shared.ResponseModels.StatisticModels
{
    public class CountsByProfessionInstitutionModel
    {
        public string ParentInstitutionName { get; set; }
        public string ProfessionName { get; set; }
        public int ProfessionCode { get; set; }
        public long Count { get; set; }
        public long MedicineCount { get; set; }
        public long DentistryCount { get; set; }
        public long PharmaceuticsCount { get; set; }
        public List<ProfessionCount> ProfessionCounts { get; set; }

        public class ProfessionCount
        {
            public ProfessionResponseDTO MyProperty { get; set; }
        }
    }
}

