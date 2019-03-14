using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Study.Core.Runtime.Server
{

    public class ServerHost : IHostedService, IDisposable
    {
        private readonly IServerBootstrap _server;
        private readonly ILogger<ServerHost> _logger;
        public ServerHost(IServerBootstrap server, ILogger<ServerHost> logger)
        {
            _server = server;
            _logger = logger;
        }
        public void Dispose()
        {
            //GC.SuppressFinalize();
            //
            //throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger?.LogInformation("服务开始断开...");
            return _server.StopAsync(cancellationToken);
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
