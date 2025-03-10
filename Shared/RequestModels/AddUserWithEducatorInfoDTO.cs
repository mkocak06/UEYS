using Shared.ResponseModels;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.RequestModels
{
    public class AddUserWithEducatorInfoDTO
    {
        public long? Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public string IdentityNo { get; set; }
        public string Nationality { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<EducatorDTO> Educators { get; set; }
        public long? InstitutionId { get; set; }
        //public long? FacultyId { get; set; }
    }
}
