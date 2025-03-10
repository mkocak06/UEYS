using System;

namespace Shared.ResponseModels
{
    public class GraduationDetailResponseDTO
    {
        public long? Id { get; set; }
        public string HigherEducationDetail { get; set; }
        public string GraduationUniversity { get; set; }
        public string GraduationFaculty { get; set; }
        public string GraduationField { get; set; }
        public string GraduationDate { get; set; }
        public bool? IsPhd { get; set; }
        public string DiplomaNumber { get; set; }
    }
}
