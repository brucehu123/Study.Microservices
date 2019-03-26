using System.Threading;
using System.Threading.Tasks;

namespace Study.Core.Runtime.Server
{
    public interface IServerBootstrap
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
