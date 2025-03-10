using Shared.Types;

namespace Shared.BaseModels
{
    public class RestartStudentModel
    {
        public long StudentId { get; set; }
        //public StudentDeleteReasonType? StudentDeleteReasonType { get; set; }
        public ReasonType? StudentDeleteReasonType { get; set; }
        public bool IsHardDeleted { get; set; }
        public bool IsDeleted { get; set; }
        public long? ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string HospitalName { get; set; }
    }
}
