using Microsoft.Extensions.Options;
using Study.Core.Consul.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Study.Core.Consul.WatcherProvider
{
    public class ClientWatchManager : IClientWatchManager
    {
        internal Dictionary<string, HashSet<Watcher>> dataWatches = new Dictionary<string, HashSet<Watcher>>();
        private readonly Timer _timer;
        private readonly ConfigInfo _config;

        public ClientWatchManager(IOptions<ConfigInfo> config)
        {
            _config = config.Value;
            var timeSpan = TimeSpan.FromSeconds(_config.WatchInterval);
            _timer = new Timer(async s =>
            {
                await Watching();
            }, null, timeSpan, timeSpan);
        }

        public Dictionary<string, HashSet<Watcher>> DataWatches
        {
            get
            {
                return dataWatches;
            }
            set
            {
                dataWatches = value;
            }
        }

        private HashSet<Watcher> Materialize()
        {
            HashSet<Watcher> result = new HashSet<Watcher>();
            lock (dataWatches)
            {
                foreach (var wa in dataWatches.Values)
                {
                    result.UnionWith(wa);
                }
            }

            return result;
        }

        private async Task Watching()
        {
            var watches = Materialize();
            foreach (var watch in watches)
            {
                await watch.Process();
            }
        }
    }
}
