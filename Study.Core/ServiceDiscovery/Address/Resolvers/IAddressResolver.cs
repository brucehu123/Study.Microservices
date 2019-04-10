using Study.Core.Address;
using System.Threading.Tasks;

namespace Study.Core.ServiceDiscovery.Address.Resolvers
{
    /// <summary>
    /// 服务地址解析器
    /// </summary>
    public interface IAddressResolver
    {
        /// <summary>
        /// 解析服务地址
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task<AddressModel> ResolverAsync(string serviceId);
    }
}
