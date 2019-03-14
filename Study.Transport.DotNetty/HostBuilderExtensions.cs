using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            });
            return builder;
        }
    }
}
