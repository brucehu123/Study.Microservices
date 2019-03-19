using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Study.ProxyGenerator.Implementation;

namespace Study.ProxyGenerator
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddClientProxy(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IServiceProxyGenerater, ServiceProxyGenerater>();
                services.AddSingleton<IServiceProxyFactory, ServiceProxyFactory>();
            });
            return builder;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddClientProxyServices(this ServiceCollection services)
        {
            services.AddSingleton<IServiceProxyGenerater, ServiceProxyGenerater>();
            services.AddSingleton<IServiceProxyFactory, ServiceProxyFactory>();
            return services;
        }
    }
}
