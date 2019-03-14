using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Study.Core.Convertibles;
using Study.Core.Convertibles.Imp;
using Study.Core.Runtime.Server;
using Study.Core.Runtime.Server.Configuration;
using Study.Core.ServiceId;
using Study.Core.ServiceId.Imp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Study.Core.Runtime.Server.Imp;


namespace Study.Core
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddRpcServer(this IHostBuilder builder)
        {

            builder.ConfigureServices((context, services) =>
            {
                //var provider = services.BuildServiceProvider();
                services.AddOptions();
                services.Configure<ServerAddress>(context.Configuration.GetSection("ServerHost"));
                services.AddSingleton<IServiceIdGenerator, ServiceIdGenerator>();
                services.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
                services.AddSingleton<ITypeConvertibleProvider, DefaultTypeConvertibleProvider>();
                services.AddSingleton<IServiceEntryProvider>(p =>
                {
                    var assemblys = DependencyContext.Default.RuntimeLibraries.SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default).Select(z => Assembly.Load(new AssemblyName(z.Name))));
                    var types = assemblys.Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes).ToArray();
                    return new DefaultServiceEntryProvider(types, p.GetRequiredService<IServerEntryCreator>(), p.GetRequiredService<ILogger>());
                });
                services.AddHostedService<ServerHost>();
                services.AddSerialization();
            });
            return builder;
        }
    }
}
