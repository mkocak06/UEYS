using Shared.ResponseModels.Authorization;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.RequestModels
{
    public class UserForRegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public string IdentityNo { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string BirthPlace { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Nationality { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public long? InstitutionId { get; set; }
        public string Password { get; set; }
        public List<long> RoleIds { get; set; }
        public long SelectedRoleId { get; set; }
        public List<ZoneRegisterDTO> Zones { get; set; }
        public virtual List<DocumentDTO> Documents { get; set; }
    }
}
