using System.ComponentModel;

namespace Shared.Types
{
    public enum ExcusedType
    {
        //ReasonType.ExcusedTransfer

        [Description("Couple Status")]
        CoupleStatus = 1,
        [Description("Health Status")]
        HealthStatus = 2,
        [Description("Negative Opinion")]
        NegativeOpinion = 3,
        [Description("Lack of Education")]
        LackofEducation = 4,
        [Description("Other")]
        Other = 5,
        [Description("Transfer after Cancellation of Negative Opinion by Court Decision")]
        TransferafterCancellationofNegativeOpinionbyCourtDecision = 6
    }
}
