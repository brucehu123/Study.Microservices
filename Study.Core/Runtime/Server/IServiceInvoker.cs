using System.Threading.Tasks;
using Study.Core.Message;
using Study.Core.Transport;

namespace Study.Core.Runtime.Server
{
    /// <summary>
    /// 本地服务调用
    /// </summary>
    public interface IServiceInvoker
    {
        Task<RemoteInvokeResultMessage> InvokerAsync(TransportMessage message);
    }
}
