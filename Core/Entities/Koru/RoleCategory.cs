﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Koru
{
    public class RoleCategory : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
