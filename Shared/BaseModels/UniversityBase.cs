namespace Shared.BaseModels;

public class UniversityBase
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string WebAddress { get; set; }
    public string Email { get; set; }
    public bool? IsPrivate { get; set; }
    public bool? IsDeleted { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }

    public long? ProvinceId { get; set; }
    public long? DistrictId { get; set; }
    public long? InstitutionId { get; set; }
}