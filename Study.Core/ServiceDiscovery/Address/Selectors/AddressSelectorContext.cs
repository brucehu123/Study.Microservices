using Study.Core.Address;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.ServiceDiscovery.Address.Selectors
{
    public class AddressSelectorContext
    {
        /// <summary>
        /// 服务描述符。
        /// </summary>
        public ServiceDescriptor Descriptor { get; set; }

        /// <summary>
        /// 服务可用地址。
        /// </summary>
        public IEnumerable<AddressModel> Address { get; set; }
    }
}
