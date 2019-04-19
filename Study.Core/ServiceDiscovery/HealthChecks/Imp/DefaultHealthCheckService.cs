using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Study.Core.Address;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Sockets;
using System.Linq;

namespace Study.Core.ServiceDiscovery.HealthChecks.Imp
{
    public class DefaultHealthCheckService : IHealthCheckService,IDisposable
    {
        private readonly ConcurrentDictionary<string, MonitorEntry> _dictionary = new ConcurrentDictionary<string, MonitorEntry>();
        private readonly Timer _timer;


        public DefaultHealthCheckService(IServiceRouteManager serviceRouteManager)
        {
            var timespan = TimeSpan.FromSeconds(10);
            _timer = new Timer(s => {
                Check(_dictionary.ToArray().Select(i => i.Value));
            }, null, timespan, timespan);

            //去除监控。
            serviceRouteManager.Removed += (s, e) =>
            {
                Remove(e.Route.Address);
            };
            //重新监控。
            serviceRouteManager.Created += (s, e) =>
            {
                var keys = e.Route.Address.Select(i => i.ToString());
                Check(_dictionary.Where(i => keys.Contains(i.Key)).Select(i => i.Value));
            };
            //重新监控。
            serviceRouteManager.Changed += (s, e) =>
            {
                var keys = e.Route.Address.Select(i => i.ToString());
                Check(_dictionary.Where(i => keys.Contains(i.Key)).Select(i => i.Value));
            };
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public Task<bool> IsHealth(AddressModel address)
        {
            return Task.Run(() =>
            {
                var key = address.ToString();
                MonitorEntry entry;

                return !_dictionary.TryGetValue(key, out entry) || entry.Health;
            });
        }

        public Task MarkFailure(AddressModel address)
        {
            return Task.Run(() =>
            {
                var key = address.ToString();
                var entry = _dictionary.GetOrAdd(key, k => new MonitorEntry(address, false));
                entry.Health = false;
            });
        }

        public Task Monitor(AddressModel address)
        {
            return Task.Run(() => { _dictionary.GetOrAdd(address.ToString(), k => new MonitorEntry(address)); });
        }

        private void Remove(IEnumerable<AddressModel> addressModels)
        {
            foreach (var addressModel in addressModels)
            {
                MonitorEntry value;
                _dictionary.TryRemove(addressModel.ToString(), out value);
            }
        }

        private static void Check(IEnumerable<MonitorEntry> entrys)
        {
            foreach (var entry in entrys)
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        socket.Connect(entry.EndPoint);
                        entry.Health = true;
                    }
                    catch 
                    {
                        entry.Health = false;
                    }
                }
            }
        }
    }
}
