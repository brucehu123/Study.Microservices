using System;
using System.Net;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Study.Core.Transport;

namespace Study.Transport.DotNetty
{
    public class ChannelClientHandlerAdpter : ChannelHandlerAdapter
    {
        private readonly Action<EndPoint> _removeClient;
        private readonly AttributeKey<EndPoint> _origEndPointKey;
        private readonly AttributeKey<IClientService> _clientServiceAttributeKey;
        private readonly ILogger _logger;

        public ChannelClientHandlerAdpter(AttributeKey<EndPoint> key, AttributeKey<IClientService> clientServiceKey, ILogger logger,/* Func<EndPoint, IRpcClient> func,*/ Action<EndPoint> action)
        {
            _logger = logger;
            //_getClient = func;
            _origEndPointKey = key;
            _clientServiceAttributeKey = clientServiceKey;
            _removeClient = action;
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"客户端通信启动成功，监听地址： {context.Channel.RemoteAddress}");
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"客户端通信断开 {context.Channel.RemoteAddress}");
            try
            {
                _removeClient(context.Channel.GetAttribute(_origEndPointKey).Get());
            }
            catch (Exception e)
            {
                ;
            }

            base.ChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var transport = (TransportMessage)message;
            var clientService = context.Channel.GetAttribute(_clientServiceAttributeKey).Get();
            clientService.RaiseReceiveAsync(transport);
            #region MyRegion
            //var transport = (TransportMessage)message;
            //var endPoint = context.Channel.GetAttribute(_origEndPointKey).Get();
            //var client = _getClient(endPoint);
            //if (client == null)
            //{
            //    throw new RpcException("无法找到活创建DotNetty客户端");
            //}

            //var callBack = client.GetCallBack(transport.Id);
            //if (transport.IsInvokeResultMessage())
            //{
            //    var result = transport.GetContent<RemoteInvokeResultMessage>();
            //    if (!string.IsNullOrEmpty(result.ExceptionMessage))
            //    {
            //        callBack.TrySetException(new RpcRemoteException(result.ExceptionMessage));
            //    }
            //    else
            //    {
            //        callBack.SetResult(transport);
            //    }
            //} 
            #endregion
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            throw exception;
        }
    }
}
