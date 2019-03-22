using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Study.Core.Transport;

namespace Study.Transport.DotNetty
{
    public delegate Task ReceivedDelegate(TransportMessage message);

    public interface IClientService
    {

        event ReceivedDelegate OnReceived;

        Task RaiseReceiveAsync(TransportMessage message);
    }
}
