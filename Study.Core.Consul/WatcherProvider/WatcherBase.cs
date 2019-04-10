using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.Consul.WatcherProvider
{
    public abstract class WatcherBase : Watcher
    {
        protected WatcherBase()
        { }

        public override async Task Process()
        {
            await ProcessImplAsync();
        }

        public abstract Task ProcessImplAsync();
    }
}
