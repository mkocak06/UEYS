using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class UserAccountDetailInfoResponseDTO
    {

        public long Id { get; set; }
        public string Email { get; set; }
        public string IdentityNo { get; set; }
        public string Name { get; set; }
        //public string Address { get; set; }
        public string Phone { get; set; }
        public string ProfilePhoto { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public virtual ProvinceResponseDTO Province { get; set; }
        public string Nationality { get; set; }
        public ForeignType? ForeignType { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? ProvinceId { get; set; }
        public long? InstitutionId { get; set; }
        public virtual InstitutionResponseDTO Institution { get; set; }
        public KPSResultResponseDTO KPSResult { get; set; }
        public virtual ICollection<EducatorResponseDTO> Educators { get; set; }
        public List<long> RoleIds { get; set; }
        public List<RoleResponseDTO> Roles { get; set; }
        public List<UserRoleResponseDTO> UserRoles { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsReadClarification { get; set; }
        public bool IsPassive { get; set; }
        public DateTime LastLoginDate { get; set; }
        public long? CountryId { get; set; }
        public long? SelectedRoleId { get; set; }
        public virtual CountryResponseDTO Country { get; set; }
        public virtual List<DocumentResponseDTO> Documents { get; set; }
        public bool? IsPhoneVerified { get; set; }
        public bool? IsMailVerified { get; set; }
    }
}
