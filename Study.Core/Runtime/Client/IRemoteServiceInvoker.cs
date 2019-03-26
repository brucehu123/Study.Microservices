using System.Threading.Tasks;
using Study.Core.Message;

namespace Study.Core.Runtime.Client
{
    public interface IRemoteServiceInvoker
    {
        Task<RemoteInvokeResultMessage> InvokeAsync(RemoteInvokeContext context);
    }
}
