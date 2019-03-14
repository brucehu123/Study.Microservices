using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using Study.Common;
using Study.Core.Runtime.Client;
using System;
using System.Text;

namespace Study.Service
{
    public class StudyServerHandler : ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = message as IByteBuffer;
            if (buffer != null)
            {
                var result = JsonConvert.DeserializeObject<RemoteInvokeContext>(buffer.ToString(Encoding.UTF8));
                var parameters = result.Parameters;

                Console.WriteLine($"接受的客户端消息,serviceId:{result.ServiceId}");

                IUserService userService = new UserService();
                var response = userService.GetUserNameAsync(int.Parse(parameters["id"].ToString())).Result;
                byte[] resultMessage = Encoding.UTF8.GetBytes(response);
                var responseMessage = Unpooled.Buffer(256);
                responseMessage.WriteBytes(resultMessage);
                context.WriteAsync(responseMessage);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("通信发生错误,错误信息: " + exception);
            context.CloseAsync();
        }
    }
}
