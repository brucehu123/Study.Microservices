using Study.Core.Address;
using System.Threading.Tasks;

namespace Study.Core.ServiceDiscovery.Address.Selectors
{
    /// <summary>
    /// 一个抽象的地址选择器。
    /// </summary>
    public interface IAddressSelector
    {
        /// <summary>
        /// 选择一个地址。
        /// </summary>
        /// <param name="context">地址选择上下文。</param>
        /// <returns>地址模型。</returns>
        ValueTask<AddressModel> SelectAsync(AddressSelectorContext context);
    }
}
