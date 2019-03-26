using Study.Core.Runtime.Server;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Codecs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Study.Core.Runtime.Server.Configuration;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels.Sockets;
using Study.Core.Transport.Codec;

namespace Study.Transport.DotNetty
{
    public class DotNettyServerBootstrap : IServerBootstrap
    {
        private readonly ILogger<DotNettyServerBootstrap> _logger;
        private readonly IOptions<ServerAddress> _address;
        private readonly ITransportMessageDecoder _decoder;
        private readonly ISocketService _service;
        private IChannel _channel;

        public DotNettyServerBootstrap(IOptions<ServerAddress> address, ISocketService service, ITransportMessageCodecFactory codecFactory, ILogger<DotNettyServerBootstrap> logger)
        {
            _logger = logger;
            _address = address;
            _decoder = codecFactory.CreateDecoder();
            _service = service;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var host = _address.Value.Host;
            var port = _address.Value.Port;

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"准备启动服务主机，监听地址：{host}:{port}。");

            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(new ChannelDecoderHandlerAdpter(_decoder,_logger));
                    pipeline.AddLast(new ChannelServerHandlerAdpter(_service, _logger));
                }));
            _channel = await bootstrap.BindAsync(new IPEndPoint(IPAddress.Parse(host), port));

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"准备启动服务主机，监听地址：{host}:{port}。");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("消息传送断开");
            return _channel.DisconnectAsync();
        }
    }
}
