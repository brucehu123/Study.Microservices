using Microsoft.Extensions.DependencyInjection;
using Study.Core.Serialization;
using Study.Core.Serialization.Imp;
using System;
using System.Collections.Generic;
using System.Text;
using Study.Core.Runtime.Server;
using Study.Core.Runtime.Server.Imp;

namespace Study.Core
{
    public static class ServiceCollectionExtensions
    {
     

        public static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer<string>, JsonSerializer>();
            services.AddSingleton<ISerializer<byte[]>, StringByteArraySerializer>();
            services.AddSingleton<ISerializer<object>, StringObjectSerializer>();

            return services;
        }

        public static IServiceCollection AddServerEntry(this IServiceCollection @this)
        {
            @this.AddSingleton<IServerEntryCreator, ServerEntryCreator>();
            @this.AddSingleton<IServiceEntryLocator, DefaultServiceEntryLocator>();

            return @this;
        }
    }
}
