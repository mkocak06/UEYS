using Shared.BaseModels;

namespace Shared.ResponseModels
{
    public class EducatorAdministrativeTitleResponseDTO : EducatorAdministrativeTitleBase
    {
        public EducatorResponseDTO Educator { get; set; }
        public TitleResponseDTO AdministrativeTitle { get; set; }
    }
}
