using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Transport.Channels;
using Study.Core.Runtime.Server;
using Study.Core.Transport.Codec;

namespace Study.Transport.DotNetty
{
  public  class ChannelDecoderHandlerAdpter: ChannelHandlerAdapter
  {
      private readonly ITransportMessageDecoder _decoder;
      private readonly IServerBootstrap _serverBootstrap;

      public ChannelDecoderHandlerAdpter(ITransportMessageDecoder decoder)
      {
          _decoder = decoder;
      }

      public override void ChannelRead(IChannelHandlerContext context, object message)
      {

      }
  }
}
