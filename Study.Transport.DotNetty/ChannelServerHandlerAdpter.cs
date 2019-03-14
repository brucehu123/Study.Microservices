using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Study.Core.Runtime;

namespace Study.Transport.DotNetty
{
    public class ChannelServerHandlerAdpter : ChannelHandlerAdapter
    {
        private readonly ISocketService _service;
        private readonly ILogger _logger;

        public ChannelServerHandlerAdpter(ISocketService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _service.OnConnected();
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _service.OnDisconnected();
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            _service.OnReceive();
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _service.OnException();
        }
    }
}
