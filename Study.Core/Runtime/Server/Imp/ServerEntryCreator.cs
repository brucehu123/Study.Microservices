using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Study.Core.Convertibles;
using Study.Core.ServiceId;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Study.Core.Attributes;

namespace Study.Core.Runtime.Server.Imp
{
    public class ServerEntryCreator : IServerEntryCreator
    {
        private readonly IServiceProvider _provider;
        private readonly IServiceIdGenerator _serviceIdGenerator;
        private readonly ITypeConvertibleService _typeConvertibleService;

        public ServerEntryCreator(IServiceProvider provider, IServiceIdGenerator serviceIdGenerator, ITypeConvertibleService typeConvertibleService)
        {
            _provider = provider;
            _serviceIdGenerator = serviceIdGenerator;
            _typeConvertibleService = typeConvertibleService;
        }

        public IEnumerable<ServerEntry> CreateServiceEntry(Type service, Type serviceImplementation)
        {
            foreach (var methodInfo in service.GetTypeInfo().GetMethods())
            {
                var implementationMethodInfo = serviceImplementation.GetTypeInfo().GetMethod(methodInfo.Name, methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                yield return Create(methodInfo, implementationMethodInfo);
            }
        }


        private ServerEntry Create(MethodInfo method, MethodBase implementationMethod)
        {
            var serviceId = _serviceIdGenerator.GenerateServiceId(method);

            var serviceDescriptor = new ServiceDescriptor
            {
                Id = serviceId
            };

            var descriptorAttributes = method.GetCustomAttributes<RpcServiceDescriptorAttribute>();
            foreach (var descriptorAttribute in descriptorAttributes)
            {
                descriptorAttribute.Apply(serviceDescriptor);
            }

            var entry = new ServerEntry()
            {
                ServiceId = serviceId,
                Descriptor = serviceDescriptor,
                Func = parameters =>
                {
                    var serviceScopeFactory = _provider.GetRequiredService<IServiceScopeFactory>();
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var instance = scope.ServiceProvider.GetRequiredService(method.DeclaringType);

                        var list = new List<object>();
                        foreach (var parameterInfo in implementationMethod.GetParameters())
                        {
                            var value = parameters[parameterInfo.Name];
                            var parameterType = parameterInfo.ParameterType;

                            var parameter = _typeConvertibleService.Convert(value, parameterType);
                            list.Add(parameter);
                        }

                        var result = implementationMethod.Invoke(instance, list.ToArray());

                        return Task.FromResult(result);
                    }
                }
            };

            return entry;
        }
    }
}
