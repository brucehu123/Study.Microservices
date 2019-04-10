using Study.Core.Address;
using Study.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.ServiceDiscovery.Imp
{
    public class DefaultServiceRouteFactory : IServiceRouteFactory
    {
        private readonly ISerializer<string> _serializer;


        public DefaultServiceRouteFactory(ISerializer<string> serializer)
        {
            _serializer = serializer;
        }


        public Task<IEnumerable<ServiceRoute>> CreateServiceRoutesAsync(IEnumerable<ServiceRouteDescriptor> descriptors)
        {
            if (descriptors == null)
                throw new ArgumentNullException(nameof(descriptors));

            descriptors = descriptors.ToArray();
            var routes = new List<ServiceRoute>(descriptors.Count());
            routes.AddRange(descriptors.Select(descriptor => new ServiceRoute
            {
                Address = CreateAddress(descriptor.AddressDescriptors),
                ServiceDescriptor = descriptor.ServiceDescriptor
            }));

            return Task.FromResult(routes.AsEnumerable());
        }

        private IEnumerable<AddressModel> CreateAddress(IEnumerable<ServiceAddressDescriptor> descriptors)
        {
            if (descriptors == null)
                yield break;

            foreach (var descriptor in descriptors)
            {
                var addressType = Type.GetType(descriptor.Type);
                yield return (AddressModel)_serializer.Deserialize(descriptor.Value, addressType);
            }
        }
    }
}
