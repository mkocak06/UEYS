using Shared.BaseModels;
using Shared.ResponseModels.Authorization;
using Shared.Types;
using System;
using System.Collections.Generic;

namespace Shared.RequestModels
{
    public class UpdateUserAccountInfoDTO : UpdateUserAccountInfoBase
    {
        public long  Id { get; set; }
        public string ProfilePhoto { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        //public string IdentityNo { get; set; }
        public string Address { get; set; }
        public ForeignType? ForeignType { get; set; }
        public long? CountryId { get; set; }
        public string Phone { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? ProvinceId { get; set; }
        public List<long> RoleIds { get; set; }
        public long? InstitutionId { get; set; }
        public List<ZoneRegisterDTO> Zones { get; set; }
        //public long? FacultyId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPassive { get; set; }
        public bool IsReadClarification { get; set; }
    }
}
