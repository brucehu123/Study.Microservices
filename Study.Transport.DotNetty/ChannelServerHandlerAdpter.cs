using System;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

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
            if(_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("通信接通");
            _service.OnConnected(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("通信断开");
            _service.OnDisconnected(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            _service.OnReceive(context,message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _service.OnException(context,exception);
        }
    }
}
