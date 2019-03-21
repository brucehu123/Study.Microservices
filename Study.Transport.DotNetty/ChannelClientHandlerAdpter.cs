using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Study.Core.Exceptions;
using Study.Core.Message;
using Study.Core.Runtime.Client;
using Study.Core.Transport;

namespace Study.Transport.DotNetty
{
    public class ChannelClientHandlerAdpter : ChannelHandlerAdapter
    {
        private readonly IRpcClient _client;
        private readonly ILogger _logger;

        public ChannelClientHandlerAdpter(IRpcClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"客户端通信启动成功，监听地址： {context.Channel.RemoteAddress}");
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"客户端通信断开 {context.Channel.RemoteAddress}");
            base.ChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var transport = (TransportMessage)message;
            var result = transport.GetContent<RemoteInvokeResultMessage>();
            if (!string.IsNullOrEmpty(result.ExceptionMessage))
            {
                _client.CallBack.TrySetException(new RpcRemoteException(result.ExceptionMessage));
            }
            else
            {
                _client.CallBack.SetResult(transport);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            throw exception;
        }
    }
}
