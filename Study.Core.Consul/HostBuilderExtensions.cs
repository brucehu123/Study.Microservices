using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Study.Core.Consul.Configuration;
using Study.Core.Consul.WatcherProvider;
using Study.Core.ServiceDiscovery;
using Study.Core.ServiceDiscovery.Imp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Consul
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseConsul(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<ConfigInfo>(context.Configuration.GetSection("Consul"));
                services.AddSingleton<IClientWatchManager, ClientWatchManager>();
                services.AddSingleton<IServiceRouteFactory, DefaultServiceRouteFactory>();
                services.AddSingleton<IServiceRouteManager, ConsulServiceRouteManager>();
            });
            return builder;
        }
    }
}
