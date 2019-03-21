using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Study.Core.Runtime.Client;
using Study.Core.Runtime.Server;

namespace Study.Transport.DotNetty
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseDotNettyServer(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IServerBootstrap, DotNettyServerBootstrap>();
                services.AddSingleton<ISocketService, DotNettySocketService>();
            });
            return builder;
        }

        public static IHostBuilder UseDotNettyClient(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IRpcClientFactory, DotNettyClientFactory>();
                });
            return builder;
        }
    }
}
