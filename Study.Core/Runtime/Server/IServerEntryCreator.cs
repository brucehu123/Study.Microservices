using System;
using System.Collections.Generic;

namespace Study.Core.Runtime.Server
{
    public interface IServerEntryCreator
    {
        IEnumerable<ServerEntry> CreateServiceEntry(Type service, Type serviceImplementation);
    }
}
