using Study.Core.ServiceDiscovery.RouteEventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Study.Core.ServiceDiscovery
{
    public interface IServiceRouteManager
    {
        /// <summary>
        /// 服务路由被创建。
        /// </summary>
        event EventHandler<ServiceRouteEventArgs> Created;

        /// <summary>
        /// 服务路由被删除。
        /// </summary>
        event EventHandler<ServiceRouteEventArgs> Removed;

        /// <summary>
        /// 服务路由被修改。
        /// </summary>
        event EventHandler<ServiceRouteChangedEventArgs> Changed;

        /// <summary>
        /// 注册路由
        /// </summary>
        /// <param name="routes"></param>
        /// <returns></returns>
        Task Register(IEnumerable<ServiceRoute> routes);
        /// <summary>
        /// 注销路由
        /// </summary>
        /// <returns></returns>
        Task Deregister();
        /// <summary>
        /// 获取所有路由
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ServiceRoute>> GetRoutes();
    }
}
