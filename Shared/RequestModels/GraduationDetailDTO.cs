using System;

namespace Shared.RequestModels
{
    public class GraduationDetailDTO
    {
        public string HigherEducationDetail { get; set; }
        public string GraduationUniversity { get; set; }
        public string GraduationFaculty { get; set; }
        public string GraduationField { get; set; }
        public string GraduationDate { get; set; }
        public string DiplomaNumber { get; set; }
        public bool? IsPhd { get; set; }
    }
}
