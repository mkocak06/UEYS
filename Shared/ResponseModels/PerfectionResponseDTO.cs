using Shared.BaseModels;
using Shared.RequestModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels
{
    public class PerfectionResponseDTO : PerfectionBase
    {
        public long? Id { get; set; }

        public PropertyResponseDTO PName { get; set; }
        public PropertyResponseDTO Group { get; set; }
        public PropertyResponseDTO Seniorty { get; set; }
        public List<PropertyResponseDTO> LevelList { get; set; }
        public List<PropertyResponseDTO> MethodList { get; set; }
        public List<CurriculumPerfectionResponseDTO> CurriculumPerfections { get; set; }
        public List<StudentPerfectionResponseDTO> StudentPerfections { get; set; }
    }
}
