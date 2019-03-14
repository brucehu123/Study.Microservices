using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Runtime.Server
{
    public  interface  IServiceEntryLocator
    {
        ServerEntry Locate(string serviceId);
    }
}
