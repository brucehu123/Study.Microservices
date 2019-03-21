using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Study.Core.Message;
using Study.Core.Runtime.Client;
using Study.Core.Transport;

namespace Study.Core.Runtime.Server.Imp
{
    /// <summary>
    /// 本地服务调用
    /// </summary>
    public class DefaultServiceInvoker : IServiceInvoker
    {
        private readonly ILogger<DefaultServiceInvoker> _logger;
        private readonly IServiceEntryLocator _entryLocator;

        public DefaultServiceInvoker(IServiceEntryLocator locator, ILogger<DefaultServiceInvoker> logger)
        {
            _entryLocator = locator;
            _logger = logger;
        }

        public async Task<RemoteInvokeResultMessage> InvokerAsync(TransportMessage message)
        {

            if (!message.IsInvokeMessage())
                return null;

            RemoteInvokeMessage remoteInvokeContext;
            try
            {
                remoteInvokeContext = message.GetContent<RemoteInvokeMessage>();
            }
            catch (Exception exception)
            {
                _logger.LogError("将接收到的消息反序列化成 TransportMessage<RemoteInvokeMessage> 时发送了错误。", exception);
                return null;
            }
            var resultMessage = new RemoteInvokeResultMessage();
            try
            {
                var entry = _entryLocator.Locate(remoteInvokeContext.ServiceId);
                var result = await entry.Func(remoteInvokeContext.Parameters);

                var task = result as Task;

                if (task == null)
                {
                    resultMessage.Result = result;
                }
                else
                {
                    task.Wait();

                    var taskType = task.GetType().GetTypeInfo();
                    if (taskType.IsGenericType)
                        resultMessage.Result = taskType.GetProperty("Result").GetValue(task);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("执行本地服务调用发生错误", e);
                resultMessage.ExceptionMessage = GetExceptionMessage(e);
            }
            return resultMessage;
        }

        private static string GetExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var message = exception.Message;
            if (exception.InnerException != null)
            {
                message += "|InnerException:" + GetExceptionMessage(exception.InnerException);
            }
            return message;
        }
    }
}
