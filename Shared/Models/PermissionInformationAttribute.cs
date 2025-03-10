using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    [AttributeUsage(AttributeTargets.All)]
    public class PermissionInformationAttribute : Attribute
    {
        public PermissionInformationAttribute(string description, string group)
        {
            this.Description = description;
            this.Group = group;
        }

        public string Description { get; set; }
        public string Group { get; set; }
    }
}
