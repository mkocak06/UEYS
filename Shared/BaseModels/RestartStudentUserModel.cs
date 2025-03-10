using System.Collections.Generic;

namespace Shared.BaseModels
{
    public class RestartStudentUserModel
    {
        public string Name { get; set; }
        public IEnumerable<RestartStudentModel> Students { get; set; }
    }
}
