using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.ServiceDiscovery
{
    public static class ServiceRouteManagerExtensions
    {
        /// <summary>
        /// 根据服务Id获取一个服务路由。
        /// </summary>
        /// <param name="serviceRouteManager">服务路由管理者。</param>
        /// <param name="serviceId">服务Id。</param>
        /// <returns>服务路由。</returns>
        public static async Task<ServiceRoute> GetAsync(this IServiceRouteManager serviceRouteManager, string serviceId)
        {
            return (await serviceRouteManager.GetRoutesAsync()).SingleOrDefault(i => i.ServiceDescriptor.Id == serviceId);
        }

    }
}
