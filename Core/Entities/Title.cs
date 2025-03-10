using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Title : BaseEntity
    {
        public string Name { get; set; }
        public TitleType TitleType { get; set; }
        public double? ContributionCoefficientForEducatorIndex { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Educator> AcademicEducators { get; set; }
        public virtual ICollection<Educator> StaffEducators { get; set; }
        public virtual ICollection<EducatorAdministrativeTitle> EducatorAdministrativeTitles { get; set; }
    }
}
