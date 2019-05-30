using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using Study.Core.Exceptions;
using Study.Core.Message;
using Study.Core.Runtime.Client;
using Study.Core.Transport;

namespace Study.Transport.DotNetty
{
    public class DotNettyClient : IRpcClient
    {
        private readonly IChannel _channel;
        private readonly IClientService _clientService;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>> _resultDictionary = new ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>>();

        public DotNettyClient(IChannel channel, IClientService clientService)
        {
            _channel = channel;
            _clientService = clientService;
            _clientService.OnReceived += message =>
            {
                return Task.Run(() =>
                {
                    TaskCompletionSource<TransportMessage> task;
                    if (!_resultDictionary.TryGetValue(message.Id, out task))
                        return;
                    if (message.IsInvokeResultMessage())
                    {
                        var content = message.GetContent<RemoteInvokeResultMessage>();
                        if (!string.IsNullOrEmpty(content.ExceptionMessage))
                        {
                            task.TrySetException(new RpcRemoteException(content.ExceptionMessage));
                        }
                        else
                        {
                            task.SetResult(message);
                        }
                    }
                });
            };
        }

        public void Dispose()
        {
            Task.Run(async () => { await _channel?.DisconnectAsync(); });
            foreach (var taskCompletionSource in _resultDictionary.Values)
            {
                taskCompletionSource.TrySetCanceled();
            }
        }

        public async Task<RemoteInvokeResultMessage> SendAsync(TransportMessage message)
        {
            var callback = RegisterCallBack(message.Id);

            try
            {
                var senderString = JsonConvert.SerializeObject(message);
                byte[] messageBytes = Encoding.UTF8.GetBytes(senderString);
                var initMessage = Unpooled.Buffer(messageBytes.Length);
                initMessage.WriteBytes(messageBytes);
                await _channel.WriteAndFlushAsync(initMessage);
            }
            catch (Exception e)
            {
                throw new RpcConnectedException("与服务端通讯时发生了异常。", e);
            }

            return await callback;

        }

        private async Task<RemoteInvokeResultMessage> RegisterCallBack(string id)
        {
            var task = new TaskCompletionSource<TransportMessage>();
            _resultDictionary.TryAdd(id, task);
            try
            {
                var result = await task.Task;
                return result.GetContent<RemoteInvokeResultMessage>();
            }
            finally
            {
                //删除回调任务
                TaskCompletionSource<TransportMessage> value;
                _resultDictionary.TryRemove(id, out value);
            }
        }
    }
}
