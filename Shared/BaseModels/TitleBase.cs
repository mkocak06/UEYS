using Shared.Types;

namespace Shared.BaseModels
{
    public class TitleBase
    {
        public string Name { get; set; }
        public TitleType TitleType { get; set; }
        public string TitleTypeName { get; set; }
    }
}
