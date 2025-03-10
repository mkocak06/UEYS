using Core.Entities.Koru;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Province : BaseEntity
    {
        public string Name { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public int Code { get;  protected set; }

        public virtual ICollection<University> Universities { get; set; }
        public virtual ICollection<Hospital> Hospitals { get; set; }
        public virtual ICollection<UserRoleProvince> UserRoleProvinces { get; set; }
        public virtual ICollection<SpecificEducationPlace> SpecificEducationPlaces { get; set; }
    }
}
