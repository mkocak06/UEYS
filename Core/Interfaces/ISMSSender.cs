using Shared.Models.SMSModels;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISMSSender
    {
        Task SendMessageAsync(SMSModel model);
        Task SendBulkMessageAsync(BulkSMSModel model);
    }
}
