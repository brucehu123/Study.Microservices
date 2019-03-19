using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Study.Core.Message;
using Study.Core.Transport;

namespace Study.Core.Runtime.Client.Imp
{
    public class RemoteServiceInvoker : IRemoteServiceInvoker
    {
        public Task<string> InvokeAsync(RemoteInvokeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.ServiceId == null)
                throw new ArgumentNullException("serviceId");
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
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
                    pipeline.AddLast(new StudyClientHandler(10, tcs, context));
                }));

            var channel = bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7788)).Result;

            return tcs.Task;
        }
    }

    public class StudyClientHandler : ChannelHandlerAdapter
    {
        private readonly int _id;
        private readonly TaskCompletionSource<string> _tsc;
        private readonly RemoteInvokeContext _remoteContext;

        public StudyClientHandler(int id, TaskCompletionSource<string> tsc, RemoteInvokeContext context)
        {
            _id = id;
            _tsc = tsc;
            _remoteContext = context;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine("客户端通信通道激活");
            Console.WriteLine("客户端发发送消息");

            if (_remoteContext == null)
                throw new Exception("RemoteContext为空");

            RemoteInvokeMessage message=new RemoteInvokeMessage()
            {
                ServiceId = _remoteContext.ServiceId,
                Parameters = _remoteContext.Parameters
            };
            var transportMessage = TransportMessage.CreateInvokeMessage(message);

             var senderString = JsonConvert.SerializeObject(transportMessage);
            byte[] messageBytes = Encoding.UTF8.GetBytes(senderString);
            var initMessage = Unpooled.Buffer(messageBytes.Length);
            initMessage.WriteBytes(messageBytes);
            context.WriteAndFlushAsync(initMessage);

            Console.WriteLine($"发送消息：{senderString}");

            Console.WriteLine("客户端消息发送完成");
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Console.WriteLine("客户端通信通道断开");
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;
            if (byteBuffer != null)
            {
                // Console.WriteLine("客户端接收到结果: " + byteBuffer.ToString(Encoding.UTF8));
                var result = byteBuffer.ToString(Encoding.UTF8);
                _tsc.SetResult(result);
            }
            //todo:实现调用
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }

    public class MessageModel
    {
        public string Type { get; set; }

        public string Method { get; set; }

        public int Id { get; set; }
    }
}
