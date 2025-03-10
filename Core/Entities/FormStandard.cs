using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FormStandard : ExtendedBaseEntity
    {

        public bool? InstitutionStatement { get; set; } //Kurum: Standart karşılanıyor mu? Evet/Hayır
        public bool? CommitteeStatement { get; set; } //Komisyon: Standart karşılanıyor mu? Evet/Hayır
        public string CommitteeDescription { get; set; }
        public long? StandardId { get; set; }
        public virtual Standard Standard { get; set; }
        public long? FormId { get; set; }
        public virtual Form Form { get; set; }
    }
}
