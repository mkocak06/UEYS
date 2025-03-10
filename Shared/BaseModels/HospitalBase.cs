namespace Shared.BaseModels;

public class HospitalBase
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string WebAddress { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public long? ManagerId { get; set; }
    public bool? IsDeleted { get; set; }
    public int Code { get; set; }
    public long? ProvinceId { get; set; }
    public long? InstitutionId { get; set; }
    public long? FacultyId { get; set; }
}