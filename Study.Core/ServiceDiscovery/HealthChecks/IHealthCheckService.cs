using Study.Core.Address;
using System.Threading.Tasks;

namespace Study.Core.ServiceDiscovery.HealthChecks
{
    /// <summary>
    /// 一个抽象的健康检查服务。
    /// </summary>
    public interface IHealthCheckService
    {
        /// <summary>
        /// 监控一个地址
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task Monitor(AddressModel address);
        /// <summary>
        /// 判断一个地址是否健康
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<bool> IsHealth(AddressModel address);
        /// <summary>
        /// 标记一个地址为不可用的
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task MarkFailure(AddressModel address);
    }
}
