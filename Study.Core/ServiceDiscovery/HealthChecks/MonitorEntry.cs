using Study.Core.Address;
using System.Net;

namespace Study.Core.ServiceDiscovery.HealthChecks
{
    public class MonitorEntry
    {
        public MonitorEntry(AddressModel address, bool health = true)
        {
            EndPoint = address.CreateEndPoint();
            Health = health;
        }

        public EndPoint EndPoint { get; set; }

        public bool Health { get; set; }
    }
}
