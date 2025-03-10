using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ResponseModels.Menu
{
    public class MenuResponseDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public int Order { get; set; }
        public long? ParentId { get; set; }
        public MenuResponseDTO ParentMenu { get; set; }
        public virtual List<MenuResponseDTO> ChildMenus { get; set; }
    }
}
