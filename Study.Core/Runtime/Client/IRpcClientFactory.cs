using System.Net;

namespace Study.Core.Runtime.Client
{
    public interface IRpcClientFactory
    {
        IRpcClient CreateClient(EndPoint endPoint);
    }
}
