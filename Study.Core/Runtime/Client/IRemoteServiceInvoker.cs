using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.Runtime.Client
{
    public interface IRemoteServiceInvoker
    {
        Task<string> InvokeAsync(RemoteInvokeContext context);
    }
}
