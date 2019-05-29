using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Study.Core.Address;
using Study.Core.Runtime.Server.Configuration;
using Study.Core.ServiceDiscovery;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Study.Core.Runtime.Server
{

    public class ServerHost : IHostedService, IDisposable
    {
        private readonly IServerBootstrap _server;
        private readonly ILogger<ServerHost> _logger;
        private readonly IServiceProvider _provider;
        private readonly IOptions<ServerAddress> _address;
        public ServerHost(IServerBootstrap server, IServiceProvider provider, IOptions<ServerAddress> address, ILogger<ServerHost> logger)
        {
            _server = server;
            _logger = logger;
            _provider = provider;
            _address = address;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var serviceRouteManager = _provider.GetService<IServiceRouteManager>();
                if (serviceRouteManager == null)
                    throw new Exception("没有启用服务路由管理功能");
                var entryProvider = _provider.GetService<IServiceEntryProvider>();
                if (entryProvider == null)
                    throw new Exception("IServiceEntryProvider初始化失败");
                var addressDescriptors = entryProvider.GetEntries().Select(i => new ServiceRoute()
                {
                    Address = new[]
                    {
                        new IpAddressModel { Ip = _address.Value.Host, Port = _address.Value.Port }
                    },

                    ServiceDescriptor = i.Descriptor
                });
                serviceRouteManager.Register(addressDescriptors);
                return _server.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError(ex, "服务启动失败");

            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger?.LogInformation("服务开始断开...");
            _server.StopAsync(cancellationToken);
            var serviceRouteManager = _provider.GetService<IServiceRouteManager>();
            return serviceRouteManager.DeregisterAsync();
        }

        public void Dispose()
        {

        }


    }

    #region MyRegion
    //public class StudyServerHandler : ChannelHandlerAdapter
    //{
    //    private IEnumerable<ServerEntry> _serverEntrys;
    //    public StudyServerHandler(IEnumerable<ServerEntry> entrys)
    //    {
    //        _serverEntrys = entrys;

    //    }
    //    public override void ChannelRead(IChannelHandlerContext context, object message)
    //    {
    //        var buffer = message as IByteBuffer;
    //        if (buffer != null)
    //        {
    //            var result = JsonConvert.DeserializeObject<RemoteInvokeContext>(buffer.ToString(Encoding.UTF8));
    //            var parameters = result.Parameters;

    //            Console.WriteLine($"接受的客户端消息,serviceId:{result.ServiceId}");

    //            var entry = _serverEntrys.SingleOrDefault(s => s.ServiceId == result.ServiceId);
    //            if (entry == null)
    //                throw new RpcRemoteException("远程服务未找到");
    //            var obj = entry.Func(parameters);
    //            var task = obj as Task;
    //            string remoteResult = string.Empty;
    //            if (task != null)
    //            {
    //                task.Wait();
    //                var taskType = task.GetType().GetTypeInfo();
    //                if (taskType.IsGenericType)
    //                    remoteResult = JsonConvert.SerializeObject(taskType.GetProperty("Result").GetValue(task));
    //            }

    //            byte[] resultMessage = Encoding.UTF8.GetBytes(remoteResult);
    //            var responseMessage = Unpooled.Buffer(256);
    //            responseMessage.WriteBytes(resultMessage);
    //            context.WriteAsync(responseMessage);
    //        }
    //    }

    //    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    //    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    //    {
    //        Console.WriteLine("通信发生错误,错误信息: " + exception);
    //        context.CloseAsync();
    //    }


    //} 
    #endregion
}
