using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using Study.Core.Runtime.Client;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using Study.Core.Transport.Codec;

namespace Study.Transport.DotNetty
{
    public class DotNettyClientFactory : IRpcClientFactory, IDisposable
    {
        private readonly ITransportMessageDecoder _decoder;
        private readonly ILogger<DotNettyClientFactory> _logger;
        private readonly ConcurrentDictionary<EndPoint, Lazy<IRpcClient>> _clients = new ConcurrentDictionary<EndPoint, Lazy<IRpcClient>>();
        private readonly Bootstrap _bootstrap;
        private static readonly AttributeKey<EndPoint> origEndPointKey = AttributeKey<EndPoint>.ValueOf(typeof(DotNettyClientFactory), nameof(EndPoint));

        private static readonly AttributeKey<IClientService> _clientServiceAttributeKey =
            AttributeKey<IClientService>.ValueOf(typeof(DotNettyClientFactory), nameof(IClientService));

        public DotNettyClientFactory(ITransportMessageCodecFactory factory, ILogger<DotNettyClientFactory> logger)
        {
            _decoder = factory.CreateDecoder();
            _logger = logger;
            _bootstrap = GetBootstrap();
            _bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(c =>
            {
                var pipeline = c.Pipeline;
                pipeline.AddLast(new LengthFieldPrepender(4));
                pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                pipeline.AddLast(new ChannelDecoderHandlerAdpter(_decoder, _logger));
                pipeline.AddLast(new ChannelClientHandlerAdpter(origEndPointKey, _clientServiceAttributeKey, _logger,/* GetOrAdd,*/ RemoveClient));
            }));
        }

        public IRpcClient CreateClientAsync(EndPoint endPoint)
        {
            var key = endPoint;
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"准备为服务端地址：{key}创建客户端。");

            return GetOrAdd(endPoint);
        }

        private IRpcClient GetOrAdd(EndPoint endPoint)
        {
            try
            {
                return _clients.GetOrAdd(endPoint, ep =>
                {
                    return new Lazy<IRpcClient>(() =>
                    {
                        var clientService = new DotNettyClientService();
                        var bootstrap = _bootstrap;
                        var channel = bootstrap.ConnectAsync(ep).Result;
                        channel.GetAttribute(origEndPointKey).Set(ep);
                        channel.GetAttribute(_clientServiceAttributeKey).Set(clientService);
                        return new DotNettyClient(channel,clientService);
                    });
                }).Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void RemoveClient(EndPoint endPoint)
        {
            _clients.TryRemove(endPoint, out var value);
        }

        private Bootstrap GetBootstrap()
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Group(new MultithreadEventLoopGroup());
            return bootstrap;
        }

        public void Dispose()
        {
            foreach (var client in _clients.Values.Where(i => i.IsValueCreated))
            {
                (client.Value as IDisposable)?.Dispose();
            }
        }
    }
}
