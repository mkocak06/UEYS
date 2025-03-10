using Shared.Types;
using System;

namespace Shared.BaseModels;

public class UserBase
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string IdentityNo { get; set; }
    public string Name { get; set; }
    public GenderType? Gender { get; set; }
    public bool? IsForeign { get; set; }
    public string FatherName { get; set; }
    public string MotherName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string BirthPlace { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Nationality { get; set; }
    public ForeignType? ForeignType { get; set; }
    public string ProfilePhoto { get; set; }
    public DateTime? LastLoginDate { get; set; } = DateTime.UtcNow;
    public DateTime? CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateDate { get; set; }
    public long? CountryId { get; set; }
    public long? InstitutionId { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsReadClarification { get; set; }
    public long? EducatorId { get; set; }
}