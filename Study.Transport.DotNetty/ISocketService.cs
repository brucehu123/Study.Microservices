using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.Transport.DotNetty
{
    public interface ISocketService
    {
        void OnConnected(IChannelHandlerContext context);
        void OnDisconnected(IChannelHandlerContext context);
        Task OnReceive(IChannelHandlerContext context, object message);


        void OnException(IChannelHandlerContext context, Exception exception);
    }
}
