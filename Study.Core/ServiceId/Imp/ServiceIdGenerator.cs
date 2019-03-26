using System;
using System.Linq;
using System.Reflection;

namespace Study.Core.ServiceId.Imp
{
    public class ServiceIdGenerator : IServiceIdGenerator
    {
        public string GenerateServiceId(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            var type = method.DeclaringType;
            if (type == null)
                throw new ArgumentNullException(nameof(method.DeclaringType), "方法定义类型不能为空");
            var id = $"{type.FullName}.{method.Name}";

            var parameters = method.GetParameters();

            if (parameters.Any())
            {
                id += "_" + string.Join("_", parameters.Select(p => p.Name));

            }

            return id;

        }
    }
}
