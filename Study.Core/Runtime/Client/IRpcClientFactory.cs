using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.Runtime.Client
{
    public interface IRpcClientFactory
    {
        IRpcClient CreateClientAsync(EndPoint endPoint);
    }
}
