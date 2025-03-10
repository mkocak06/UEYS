using System.ComponentModel;

namespace Shared.Types
{
    public enum ExitExamResultType
    {
        [Description("Sınav Sonuçlandırıldı")]
        Concluded,
        [Description("Sınav Sonuçlandırılmadı")]
        NotConcluded
    }
}
