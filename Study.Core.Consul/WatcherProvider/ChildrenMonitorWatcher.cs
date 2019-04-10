using Consul;
using Study.Core.Consul.Utilitys;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Study.Core.Consul.WatcherProvider
{
    public class ChildrenMonitorWatcher : WatcherBase
    {
        private string[] _currentData = new string[0];
        private readonly IClientWatchManager _clientWatchManager;
        private readonly string _path;
        private readonly ConsulClient _consul;
        private readonly Action<string[], string[]> _action;
        private readonly Func<string[], string[]> _func;

        public ChildrenMonitorWatcher(IClientWatchManager clientWatchManager, ConsulClient consul, Action<string[], string[]> action, Func<string[], string[]> func, string path)
        {
            this._clientWatchManager = clientWatchManager;
            this._path = path;
            this._consul = consul;
            this._action = action;
            this._func = func;
            RegisterWatch();
        }

        public ChildrenMonitorWatcher SetCurrentData(string[] currentData)
        {
            _currentData = currentData ?? new string[0];
            return this;
        }

        public override async Task ProcessImplAsync()
        {
            RegisterWatch(this);

            var result = await _consul.GetChildrenAsync(_path);
            if (result != null)
            {
                var convertResult = _func.Invoke(result).Select(key => $"{_path}{key}").ToArray();
                _action(_currentData, convertResult);
                this.SetCurrentData(convertResult);
            }
        }

        private void RegisterWatch(Watcher watcher = null)
        {
            ChildWatchRegistration wcb = new ChildWatchRegistration(_clientWatchManager, watcher == null ? this : watcher, _path);
            wcb.Register();
        }
    }
}
