using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Study.Core.Address;
using Study.Core.ServiceDiscovery.HealthChecks;
using Study.Core.ServiceDiscovery.RouteEventArgs;

namespace Study.Core.ServiceDiscovery.Address.Selectors.Imp
{
    public class PollingAddressSelector : AddressSelectorBase
    {
        private readonly IHealthCheckService _healthCheckService;
        private readonly ConcurrentDictionary<string, Lazy<AddressEntry>> _concurrent = new ConcurrentDictionary<string, Lazy<AddressEntry>>();

        public PollingAddressSelector(IServiceRouteManager serviceRouteManager, IHealthCheckService healthCheckService)
        {
            this._healthCheckService = healthCheckService;
            serviceRouteManager.Changed += ServiceRouteManager_Changed;
            serviceRouteManager.Removed += ServiceRouteManager_Removed;
        }

        protected override async Task<AddressModel> SelectAsync(AddressSelectorContext context)
        {
            var key = GetCacheKey(context.Descriptor);
            var entry = _concurrent.GetOrAdd(key,
                k => new Lazy<AddressEntry>(() => new AddressEntry(context.Address))).Value;

            AddressModel address;
            do
            {
                address = entry.GetAddress();
            } while (await _healthCheckService.IsHealth(address) == false);

            return address;

        }

        private void ServiceRouteManager_Removed(object sender, ServiceRouteEventArgs e)
        {
            var key = GetCacheKey(e.Route.ServiceDescriptor);
            Lazy<AddressEntry> value;
            _concurrent.TryRemove(key, out value);
        }

        private void ServiceRouteManager_Changed(object sender, ServiceRouteChangedEventArgs e)
        {
            var key = GetCacheKey(e.Route.ServiceDescriptor);

            var addresses = e.Route.Address;
           
            if (!_concurrent.TryUpdate(key, new Lazy<AddressEntry>(() => new AddressEntry(addresses)), new Lazy<AddressEntry>(() => new AddressEntry(e.OldRoute.Address))))
            {
                if (_concurrent.TryRemove(key, out var value))
                    _concurrent.TryAdd(key, new Lazy<AddressEntry>(() => new AddressEntry(addresses)));
            }

        }

        private static string GetCacheKey(ServiceDescriptor descriptor)
        {
            return descriptor.Id;
        }

        #region 帮助类
        protected class AddressEntry
        {
            #region 私有成员
            private int _index;
            private int _lock;
            private readonly int _maxIndex;
            private readonly AddressModel[] _address;
            #endregion

            #region 构造
            public AddressEntry(IEnumerable<AddressModel> address)
            {
                this._address = address.ToArray();
                this._maxIndex = _address.Length - 1;
            }
            #endregion

            #region 方法
            public AddressModel GetAddress()
            {
                while (true)
                {
                    //无法获得锁，则等待一段时间
                    if (Interlocked.Exchange(ref _lock, 1) != 0)
                    {
                        default(SpinWait).SpinOnce();
                        continue;
                    }

                    var address = _address[_index];

                    //如果当前指标等于数组最大值，则从新开始获取数组元素
                    if (_maxIndex > _index)
                        _index++;
                    else
                        _index = 0;
                    //释放锁
                    Interlocked.Exchange(ref _lock, 0);
                    return address;
                }

            }
            #endregion

        }
        #endregion

    }


}
