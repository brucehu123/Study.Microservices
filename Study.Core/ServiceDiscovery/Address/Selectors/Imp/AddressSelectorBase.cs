using System;
using System.Threading.Tasks;
using Study.Core.Address;
using System.Linq;

namespace Study.Core.ServiceDiscovery.Address.Selectors.Imp
{
    public abstract class AddressSelectorBase : IAddressSelector
    {
        #region Implementation of IAddressSelector

        /// <summary>
        /// 选择一个地址。
        /// </summary>
        /// <param name="context">地址选择上下文。</param>
        /// <returns>地址模型。</returns>
        ValueTask<AddressModel> IAddressSelector.SelectAsync(AddressSelectorContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Descriptor == null)
                throw new ArgumentNullException(nameof(context.Descriptor));
            if (context.Address == null)
                throw new ArgumentNullException(nameof(context.Address));

            var address = context.Address.ToArray();
            if (!address.Any())
                throw new ArgumentException("没有任何地址信息。", nameof(context.Address));

            return context.Address.Count() == 1 ? new ValueTask<AddressModel>(context.Address.First()) : new ValueTask<AddressModel>(SelectAsync(context));
        }

        #endregion Implementation of IAddressSelector

        /// <summary>
        /// 选择一个地址。
        /// </summary>
        /// <param name="context">地址选择上下文。</param>
        /// <returns>地址模型。</returns>
        protected abstract Task<AddressModel> SelectAsync(AddressSelectorContext context);
    }
}
