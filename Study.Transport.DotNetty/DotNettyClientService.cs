using System.Threading.Tasks;
using Study.Core.Transport;

namespace Study.Transport.DotNetty
{
    public class DotNettyClientService : IClientService
    {
        public event ReceivedDelegate OnReceived;

        public Task RaiseReceiveAsync(TransportMessage message)
        {
            return OnReceived?.Invoke(message);
        }
    }
}
