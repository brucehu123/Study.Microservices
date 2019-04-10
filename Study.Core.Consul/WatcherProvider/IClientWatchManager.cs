using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Consul.WatcherProvider
{
    public interface IClientWatchManager
    {
        Dictionary<string, HashSet<Watcher>> DataWatches { get; set; }
    }
}
