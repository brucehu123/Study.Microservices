using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Study.Core.Runtime.Server;
using Study.Core.Transport.Codec;

namespace Study.Transport.DotNetty
{
    public class ChannelDecoderHandlerAdpter : ChannelHandlerAdapter
    {
        private readonly ITransportMessageDecoder _decoder;
        private readonly ILogger _logger;

        public ChannelDecoderHandlerAdpter(ITransportMessageDecoder decoder, ILogger logger)
        {
            _decoder = decoder;
            _logger = logger;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer) message;
            var data = buffer.ToString(Encoding.UTF8);
            var result = _decoder.Decoder(data);
            context.FireChannelRead(result);
        }
    }
}
