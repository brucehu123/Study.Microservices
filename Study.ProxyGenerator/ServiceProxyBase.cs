using Study.Core.Runtime.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Study.Core.Convertibles;
using Study.Core.Message;

namespace Study.ProxyGenerator
{
    public abstract class ServiceProxyBase
    {
        private readonly IRemoteServiceInvoker _remoteServiceInvoker;
        private readonly ITypeConvertibleService _typeConvertibleService;
        public ServiceProxyBase(IRemoteServiceInvoker remoteServiceInvoker, ITypeConvertibleService typeConvertibleService)
        {
            this._remoteServiceInvoker = remoteServiceInvoker;
            this._typeConvertibleService = typeConvertibleService;
        }

        public async Task<T> Invoke<T>(IDictionary<string, object> parameters, string serviceId)
        {
            var context = new RemoteInvokeContext()
            {
                ServiceId = serviceId,
                Parameters = parameters
            };
            RemoteInvokeResultMessage message;
            try
            {
                 message = await _remoteServiceInvoker.InvokeAsync(context);
            }
            catch (Exception e)
            {
                //todo:log
                throw;
            }

            if (message == null)
                return default(T);
            var result = _typeConvertibleService.Convert(message.Result, typeof(T));

            return (T)result;
        }

        protected async Task Invoke(IDictionary<string, object> parameters, string serviceId)
        {
            try
            {
                await _remoteServiceInvoker.InvokeAsync(new RemoteInvokeContext
                {
                    ServiceId = serviceId,
                    Parameters = parameters
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
