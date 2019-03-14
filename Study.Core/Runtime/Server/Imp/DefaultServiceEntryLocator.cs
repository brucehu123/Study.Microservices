using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Study.Core.Runtime.Server.Imp
{
    public class DefaultServiceEntryLocator : IServiceEntryLocator
    {
        private readonly ConcurrentDictionary<string, ServerEntry> _entryCache =
            new ConcurrentDictionary<string, ServerEntry>();

        private readonly ILogger<DefaultServiceEntryLocator> _logger;
        public DefaultServiceEntryLocator(IServiceEntryProvider provider, ILogger<DefaultServiceEntryLocator> logger)
        {
            _logger = logger;
            Initialize(provider);
        }

        public ServerEntry Locate(string serviceId)
        {
            return _entryCache.TryGetValue(serviceId, out var entry) ? entry : null;
        }

        private void Initialize(IServiceEntryProvider provider)
        {
            var entries = provider.GetEntries();
            if (entries != null && entries.Any())
            {
                foreach (var entry in entries)
                {
                    _entryCache.AddOrUpdate(entry.ServiceId, entry, (k, v) => entry);
                }
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("没有发现可注册的服务");
            }
        }
    }
}
