using System.Reflection;

namespace Study.Core.ServiceId
{
    public interface IServiceIdGenerator
    {
        string GenerateServiceId(MethodInfo method);
    }
}
