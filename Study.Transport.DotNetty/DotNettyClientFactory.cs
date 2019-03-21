using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs;
using Study.Core.Runtime.Client;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Study.Core.Runtime.Client.Imp;
using Study.Core.Transport.Codec;

namespace Study.Transport.DotNetty
{
    public class DotNettyClientFactory : IRpcClientFactory
    {
        private readonly ITransportMessageDecoder _decoder;
        private readonly ILogger<DotNettyClientFactory> _logger;

        public DotNettyClientFactory(ITransportMessageCodecFactory factory, ILogger<DotNettyClientFactory> logger)
        {
            _decoder = factory.CreateDecoder();
            _logger = logger;
        }

        public async Task<IRpcClient> CreateClientAsync(IPEndPoint endPoint)
        {
            var client = new DotNettyClient();
            var bootstrap = new Bootstrap();
            bootstrap
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Group(new MultithreadEventLoopGroup())
                .Handler(new ActionChannelInitializer<ISocketChannel>(c =>
                {
                    var pipeline = c.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(new ChannelDecoderHandlerAdpter(_decoder, _logger));
                    pipeline.AddLast(new ChannelClientHandlerAdpter(client, _logger));
                }));

            client.Channel = await bootstrap.ConnectAsync(endPoint);
          
            return client;
        }
    }
}
