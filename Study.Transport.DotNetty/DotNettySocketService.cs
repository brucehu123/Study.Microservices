using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Study.Core.Exceptions;
using Study.Core.Runtime;
using Study.Core.Runtime.Server;
using Study.Core.Transport;
using Study.Core.Transport.Codec;

namespace Study.Transport.DotNetty
{
    public class DotNettySocketService : ISocketService
    {
        private readonly IServiceInvoker _invoker;
        private readonly ITransportMessageEncoder _encoder;
        private readonly ILogger<DotNettySocketService> _logger;

        public DotNettySocketService(IServiceInvoker invoker, ITransportMessageCodecFactory codecFactory, ILogger<DotNettySocketService> logger)
        {
            _encoder = codecFactory.CreateEncoder();
            _invoker = invoker;
            _logger = logger;
        }

        //private executor
        public void OnConnected(IChannelHandlerContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"客户端链接成功,服务地址:{context.Channel.LocalAddress},客户端地址:{context.Channel.RemoteAddress}");
        }

        public void OnDisconnected(IChannelHandlerContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"客户端链接断开,服务地址:{context.Channel.LocalAddress},客户端地址:{context.Channel.RemoteAddress}");
        }

        public void OnException(IChannelHandlerContext context, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void OnReceive(IChannelHandlerContext context, object message)
        {
            var data = (TransportMessage)message;

            Task.Run(async () =>
            {

                var result = await _invoker.InvokerAsync(data);
                if (result == null)
                {
                    throw new RpcRemoteException("本地服务调用失败", null);
                }

                var transportMessage = new TransportMessage(result);
                var byteMessage = _encoder.Encode(transportMessage);
                var buffer = Unpooled.Buffer(byteMessage.Length, byteMessage.Length);
                buffer.WriteBytes(byteMessage);
                await context.WriteAndFlushAsync(buffer);

            }).ConfigureAwait(false);


            //todo: send result to remote client
        }
    }
}
