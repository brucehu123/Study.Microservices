using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.Consul.WatcherProvider
{
    public abstract class Watcher
    {
        protected Watcher()
        { }

        public abstract Task Process();
    }
}
