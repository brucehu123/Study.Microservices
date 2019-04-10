using System.Threading.Tasks;
using Study.Core.Address;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using Study.Core.ServiceDiscovery.Address.Selectors;
using Study.Core.ServiceDiscovery.HealthChecks;

namespace Study.Core.ServiceDiscovery.Address.Resolvers.Imp
{
    public class DefaultAddressResolver : IAddressResolver
    {
        private readonly ILogger<DefaultAddressResolver> _logger;
        private readonly IServiceRouteManager _manager;
        private readonly IAddressSelector _selector;
        private readonly IHealthCheckService _healthChecksService;

        public DefaultAddressResolver(IServiceRouteManager manager, IAddressSelector selector, IHealthCheckService healthCheckService, ILogger<DefaultAddressResolver> logger)
        {
            _manager = manager;
            _selector = selector;
            _healthChecksService = healthCheckService;
            _logger = logger;
        }

        public async Task<AddressModel> ResolverAsync(string serviceId)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"正在准备为服务{serviceId}解析地址");

            var serviceRoutes = await _manager.GetRoutesAsync();

            var serviceRoute = serviceRoutes.Where(s => s.ServiceDescriptor.Id == serviceId).FirstOrDefault();

            if (serviceRoute == null)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning($"服务{serviceId}没有找到对应的服务");
                return null;
            }

            var address = new List<AddressModel>();

            foreach (var am in serviceRoute.Address)
            {
                await _healthChecksService.Monitor(am);
                if (!await _healthChecksService.IsHealth(am))
                    continue;
                address.Add(am);
            }

            if (!address.Any())
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning($"服务{serviceId}没有找到对应的服务");
                return null;
            }
            else
            {
                //重新赋值ServiceRoute,使Address集合始终有效
                return await _selector.SelectAsync(new ServiceRoute()
                {
                    ServiceDescriptor = serviceRoute.ServiceDescriptor,
                    Address = address
                });
            }

        }
    }
}
