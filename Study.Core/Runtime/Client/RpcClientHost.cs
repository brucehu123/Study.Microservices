using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Study.Core.Runtime.Client
{
    public class RpcClientHost : IHostedService
    {
        private readonly ILogger<RpcClientHost> _logger;

        public RpcClientHost(ILogger<RpcClientHost> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("客户端程序启动");
            return  Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug("客户端程序停止");
            return Task.CompletedTask;
        }
    }
}
