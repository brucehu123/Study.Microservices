using Study.Core.Runtime.Client;
using System;
using System.Linq;
using System.Reflection;

namespace Study.ProxyGenerator.Implementation
{
    /// <summary>
    /// 默认的服务代理工厂实现。
    /// </summary>
    public class ServiceProxyFactory : IServiceProxyFactory
    {
        #region Field

        private readonly IRemoteServiceInvoker _remoteServiceInvoker;

        #endregion Field

        #region Constructor

        public ServiceProxyFactory(IRemoteServiceInvoker remoteServiceInvoker)
        {
            _remoteServiceInvoker = remoteServiceInvoker;
        }

        #endregion Constructor

        #region Implementation of IServiceProxyFactory

        /// <summary>
        /// 创建服务代理。
        /// </summary>
        /// <param name="proxyType">代理类型。</param>
        /// <returns>服务代理实例。</returns>
        public object CreateProxy(Type proxyType)
        {
            var instance = proxyType.GetTypeInfo().GetConstructors().First().Invoke(new object[] { _remoteServiceInvoker });
            return instance;
        }

        #endregion Implementation of IServiceProxyFactory
    }
}
