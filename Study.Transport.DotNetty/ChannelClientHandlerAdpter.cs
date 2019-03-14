using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Transport.DotNetty
{
    public class ChannelClientHandlerAdpter : ChannelHandlerAdapter
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
        }
    }
}
