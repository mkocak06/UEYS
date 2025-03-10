namespace Shared.BaseModels;

public class FacultyBase
{
    public long? Id { get; set; }
    public long? UniversityId { get; set; }
    public long? ProfessionId { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Name { get; set; }

}