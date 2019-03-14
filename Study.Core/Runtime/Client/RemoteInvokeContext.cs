using Study.Core.Address;
using System.Collections.Generic;

namespace Study.Core.Runtime.Client
{
    public class RemoteInvokeContext
    {
        public string ServiceId { get; set; }

       public IDictionary<string, object> Parameters { get; set; }
    }
}
