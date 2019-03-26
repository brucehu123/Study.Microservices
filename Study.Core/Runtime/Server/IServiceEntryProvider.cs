using System.Collections.Generic;

namespace Study.Core.Runtime.Server
{
    public  interface IServiceEntryProvider
    {
        IEnumerable<ServerEntry> GetEntries();
    }
}
