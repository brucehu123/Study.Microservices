using System.Net;

namespace Study.Core.Runtime.Client
{
    public interface IRpcClientFactory
    {
        IRpcClient CreateClientAsync(EndPoint endPoint);
    }
}
