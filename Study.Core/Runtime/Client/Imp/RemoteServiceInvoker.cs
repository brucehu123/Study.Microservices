using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Study.Core.Message;
using Study.Core.Transport;

namespace Study.Core.Runtime.Client.Imp
{
    public class RemoteServiceInvoker : IRemoteServiceInvoker
    {
        private readonly IRpcClientFactory _clientFactory;
        private readonly ILogger<RemoteServiceInvoker> _logger;

        public RemoteServiceInvoker(IRpcClientFactory factory, ILogger<RemoteServiceInvoker> logger)
        {
            this._logger = logger;
            this._clientFactory = factory;
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

            //todo: 地址改成服务发现
            //todo: 添加断路器（polly）
            var client = _clientFactory.CreateClientAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7788));

            try
            {
              return  await client.SendAsync(transportMessage);
            }
            catch (Exception e)
            {
                _logger.LogError("发送远程消息错误", e);
                throw e;
            }
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
