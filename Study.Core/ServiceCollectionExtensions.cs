using Microsoft.Extensions.DependencyInjection;
using Study.Core.Serialization;
using Study.Core.Serialization.Imp;
using System;
using System.Collections.Generic;
using System.Text;
using Study.Core.Runtime.Server;
using Study.Core.Runtime.Server.Imp;
using Study.Core.Transport.Codec;
using Study.Core.Transport.Codec.Imp;

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

        public static IServiceCollection AddServerEntry(this IServiceCollection services)
        {
            services.AddSingleton<IServerEntryCreator, ServerEntryCreator>();
            services.AddSingleton<IServiceEntryLocator, DefaultServiceEntryLocator>();

            return services;
        }

        public static IServiceCollection AddCodec(this IServiceCollection services)
        {
            services.AddSingleton<ITransportMessageCodecFactory, DefaultTransportMessageCodecFactory>();
            return services;
        }
    }
}
