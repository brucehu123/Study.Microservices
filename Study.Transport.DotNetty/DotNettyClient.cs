using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using Study.Core.Message;
using Study.Core.Runtime.Client;
using Study.Core.Transport;

namespace Study.Transport.DotNetty
{
    public class DotNettyClient : IRpcClient
    {

        private IChannel _channel;
        private readonly TaskCompletionSource<TransportMessage> _tsc;

        public DotNettyClient()
        {
            _tsc = new TaskCompletionSource<TransportMessage>();
        }

        public IChannel Channel
        {
            get => _channel;
            set => _channel = value;
        }

        TaskCompletionSource<TransportMessage> IRpcClient.CallBack => _tsc;

        public Task SendAsync(TransportMessage message)
        {
            var senderString = JsonConvert.SerializeObject(message);
            byte[] messageBytes = Encoding.UTF8.GetBytes(senderString);
            var initMessage = Unpooled.Buffer(messageBytes.Length);
            initMessage.WriteBytes(messageBytes);
            return _channel.WriteAndFlushAsync(initMessage);
        }
    }
}
