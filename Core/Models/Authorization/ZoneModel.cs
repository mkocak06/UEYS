using Core.Entities;
using Core.Entities.Koru;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Authorization
{
    public class ZoneModel
    {
        public RoleCategoryType RoleCategory { get; set; }
        public List<Faculty> Faculties { get; set; }
        public List<Hospital> Hospitals { get; set; }
        public List<Program> Programs { get; set; }
        public List<Province> Provinces { get; set; }
        public List<University> Universities { get; set; }
        public List<Student> Students { get; set; }
    }
}
