using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Study.Core.Attributes;

namespace Study.Core.Runtime.Server.Imp
{
    public class DefaultServiceEntryProvider : IServiceEntryProvider
    {
        private readonly IEnumerable<Type> _types;
        private readonly IServerEntryCreator _entryCreator;
        private readonly ILogger _logger;

        public DefaultServiceEntryProvider(IEnumerable<Type> types, IServerEntryCreator entryCreator, ILogger logger)
        {
            _types = types;
            _entryCreator = entryCreator;
            _logger = logger;
        }

        public IEnumerable<ServerEntry> GetEntries()
        {
            var services = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsInterface && typeInfo.GetCustomAttribute<RpcServiceBundleAttribute>() != null;
            }).ToArray();
            var serviceImplementations = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsClass && !typeInfo.IsAbstract && i.Namespace != null && !i.Namespace.StartsWith("System") &&
                       !i.Namespace.StartsWith("Microsoft");
            }).ToArray();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"发现了以下服务：{string.Join(",", services.Select(i => i.ToString()))}。");
            }

            var entries = new List<ServerEntry>();
            foreach (var service in services)
            {
                foreach (var serviceImplementation in serviceImplementations.Where(i => service.GetTypeInfo().IsAssignableFrom(i)))
                {
                    entries.AddRange(_entryCreator.CreateServiceEntry(service, serviceImplementation));
                }
            }
            return entries;
        }
    }
}
