using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.BaseModels;


namespace Shared.RequestModels
{
    public class UserDTO : UserBase
    {
        public virtual StudentDTO Student { get; set; }
        public virtual EducatorDTO Educator { get; set; }
    }
}
