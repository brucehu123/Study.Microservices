using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Study.Core.Message;
using Study.Core.Transport;
using Study.Core.ServiceDiscovery.Address.Resolvers;
using Study.Core.ServiceDiscovery.HealthChecks;
using Polly;
using Study.Core.Exceptions;
using Study.Core.Address;

namespace Study.Core.Runtime.Client.Imp
{
    public class RemoteServiceInvoker : IRemoteServiceInvoker
    {
        private readonly IRpcClientFactory _clientFactory;
        private readonly IAddressResolver _addressResolver;
        private readonly IHealthCheckService _healthCheckService;
        private readonly ILogger<RemoteServiceInvoker> _logger;

        public RemoteServiceInvoker(IRpcClientFactory factory, IAddressResolver addressResolver, IHealthCheckService healthCheckService, ILogger<RemoteServiceInvoker> logger)
        {
            this._logger = logger;
            this._clientFactory = factory;
            this._healthCheckService = healthCheckService;
            this._addressResolver = addressResolver;
        }

        public async Task<RemoteInvokeResultMessage> InvokeAsync(RemoteInvokeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.ServiceId == null)
                throw new ArgumentNullException("serviceId");

            RemoteInvokeMessage message = new RemoteInvokeMessage()
            {
                ServiceId = context.ServiceId,
                Parameters = context.Parameters
            };
            var transportMessage = TransportMessage.CreateInvokeMessage(message);


            var retryPolicy = Policy.Handle<RpcConnectedException>()
                .Or<RpcRemoteException>()
                .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                               , async (ex, time, i, ctx) =>
                                {
                                    var address = ctx["address"] as AddressModel;
                                    if (_logger.IsEnabled(LogLevel.Debug))
                                        _logger.LogDebug($"第{i}次重试，重试时间间隔{time.Seconds},发生时间:{DateTime.Now},地址:{address.ToString()}");
                                    await _healthCheckService.MarkFailure(address);
                                });


            #region MyRegion
            //var fallBackPolicy = Policy<TransportMessage>.Handle<Exception>().Fallback(new TransportMessage());
            //var mixPolicy = Policy.Wrap(fallBackPolicy, retryPolicy); 
            //var breakerPolicy=Policy.Handle<RpcConnectedException>()
            //                    .CircuitBreakerAsync(5,TimeSpan.FromSeconds(10),)
            #endregion

            try
            {
                var policyContext = new Context();
                return await retryPolicy.ExecuteAsync<RemoteInvokeResultMessage>(ctx =>
                 {
                     return RetryExectueAsync(ctx, _addressResolver, context.ServiceId, transportMessage);
                 }, policyContext);

            }
            catch (Exception e)
            {
                _logger.LogError("发送远程消息错误", e);
                throw;
            }
        }

        private async Task<RemoteInvokeResultMessage> RetryExectueAsync(Context ctx, IAddressResolver resolver, string serviceId, TransportMessage transportMessage)
        {
            var address = await resolver.ResolverAsync(serviceId);
            ctx["address"] = address;
            var client = _clientFactory.CreateClient(address.CreateEndPoint());
            return await client.SendAsync(transportMessage);
        }
    }

    #region MyRegion
    //public class StudyClientHandler : ChannelHandlerAdapter
    //{
    //    private readonly int _id;
    //    private readonly TaskCompletionSource<string> _tsc;
    //    private readonly RemoteInvokeContext _remoteContext;

    //    public StudyClientHandler(int id, TaskCompletionSource<string> tsc, RemoteInvokeContext context)
    //    {
    //        _id = id;
    //        _tsc = tsc;
    //        _remoteContext = context;
    //    }

    //    public override void ChannelActive(IChannelHandlerContext context)
    //    {
    //        Console.WriteLine("客户端通信通道激活");
    //        Console.WriteLine("客户端发发送消息");

    //        if (_remoteContext == null)
    //            throw new Exception("RemoteContext为空");

    //        RemoteInvokeMessage message = new RemoteInvokeMessage()
    //        {
    //            ServiceId = _remoteContext.ServiceId,
    //            Parameters = _remoteContext.Parameters
    //        };
    //        var transportMessage = TransportMessage.CreateInvokeMessage(message);

    //        var senderString = JsonConvert.SerializeObject(transportMessage);
    //        byte[] messageBytes = Encoding.UTF8.GetBytes(senderString);
    //        var initMessage = Unpooled.Buffer(messageBytes.Length);
    //        initMessage.WriteBytes(messageBytes);
    //        context.WriteAndFlushAsync(initMessage);

    //        Console.WriteLine($"发送消息：{senderString}");

    //        Console.WriteLine("客户端消息发送完成");
    //    }

    //    public override void ChannelInactive(IChannelHandlerContext context)
    //    {
    //        Console.WriteLine("客户端通信通道断开");
    //    }

    //    public override void ChannelRead(IChannelHandlerContext context, object message)
    //    {
    //        var byteBuffer = message as IByteBuffer;
    //        if (byteBuffer != null)
    //        {
    //            var result = byteBuffer.ToString(Encoding.UTF8);
    //            _tsc.SetResult(result);
    //        }
    //    }

    //    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    //    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    //    {
    //        Console.WriteLine("Exception: " + exception);
    //        context.CloseAsync();
    //    }
    //} 
    #endregion
}
