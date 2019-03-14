using Study.Core.Address;
using Study.Core.Runtime.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.ProxyGenerator
{
    public abstract class ServiceProxyBase
    {
        private readonly IRemoteServiceInvoker _remoteServiceInvoker;
        public ServiceProxyBase(IRemoteServiceInvoker remoteServiceInvoker)
        {
            this._remoteServiceInvoker = remoteServiceInvoker;
        }

        public async Task<string> Invoke(IDictionary<string, object> parameters, string serviceId)
        {
            var context = new RemoteInvokeContext() {
                ServiceId=serviceId,
                Parameters=parameters
            };

            return await _remoteServiceInvoker.InvokeAsync(context);
        }
    }
}
