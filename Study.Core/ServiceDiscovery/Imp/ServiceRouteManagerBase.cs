using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Study.Core.ServiceDiscovery.RouteEventArgs;
using Study.Core.Serialization;
using System.Linq;

namespace Study.Core.ServiceDiscovery.Imp
{
    public abstract class ServiceRouteManagerBase : IServiceRouteManager
    {
        private readonly ISerializer<string> _serializer;
        private EventHandler<ServiceRouteEventArgs> _created;
        private EventHandler<ServiceRouteEventArgs> _removed;
        private EventHandler<ServiceRouteChangedEventArgs> _changed;

        public ServiceRouteManagerBase(ISerializer<string> serializer)
        {
            _serializer = serializer;
        }

        /// <summary>
        /// 路由创建
        /// </summary>
        public event EventHandler<ServiceRouteEventArgs> Created
        {
            add { _created += value; }
            remove { _created -= value; }
        }
        /// <summary>
        /// 路由删除
        /// </summary>
        public event EventHandler<ServiceRouteEventArgs> Removed
        {
            add { _removed += value; }
            remove { _removed -= value; }
        }
        /// <summary>
        /// 路由变化
        /// </summary>
        public event EventHandler<ServiceRouteChangedEventArgs> Changed
        {
            add { _changed += value; }
            remove { _changed -= value; }
        }

        public abstract Task DeregisterAsync();


        public abstract Task<IEnumerable<ServiceRoute>> GetRoutesAsync();


        public virtual Task Register(IEnumerable<ServiceRoute> routes)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));

            var descriptors = routes.Where(route => route != null).Select(route => new ServiceRouteDescriptor
            {
                AddressDescriptors = route.Address?.Select(address => new ServiceAddressDescriptor
                {
                    Type = address.GetType().FullName,
                    Value = _serializer.Serialize(address)
                }) ?? Enumerable.Empty<ServiceAddressDescriptor>(),
                ServiceDescriptor = route.ServiceDescriptor
            });

            return SetRoutesAsync(descriptors);
        }

        public abstract Task SetRoutesAsync(IEnumerable<ServiceRouteDescriptor> descriptors);


        protected void OnCreated(params ServiceRouteEventArgs[] args)
        {
            if (_created == null)
                return;

            foreach (var arg in args)
                _created(this, arg);
        }

        protected void OnChanged(params ServiceRouteChangedEventArgs[] args)
        {
            if (_changed == null)
                return;

            foreach (var arg in args)
                _changed(this, arg);
        }

        protected void OnRemoved(params ServiceRouteEventArgs[] args)
        {
            if (_removed == null)
                return;

            foreach (var arg in args)
                _removed(this, arg);
        }

    }
}
