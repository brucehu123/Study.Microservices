using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Study.Core.ServiceId
{
    public interface IServiceIdGenerator
    {
        string GenerateServiceId(MethodInfo method);
    }
}
