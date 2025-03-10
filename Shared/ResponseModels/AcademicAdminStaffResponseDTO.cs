using System;
using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class AcademicAdminStaffResponseDTO
    {
        public long? AcademicTitleId { get; set; }
        public string AcademicTitleName{ get; set; }
        public List<AcademicAdminStaffAssignmentDTO> Assignment { get; set; }
    }

    public class AcademicAdminStaffAssignmentDTO
    {
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public long? GorevBirimId { get; set; }
        public long? GorevTurId { get; set; }
    }
}
