using System;
using System.Threading.Tasks;
using Study.Core.Address;
using System.Linq;

namespace Study.Core.ServiceDiscovery.Address.Selectors.Imp
{
    public class RandomAddressSelector : AddressSelectorBase
    {
        private Func<int, int, int> _generate;
        private readonly Random _random;

        /// <summary>
        /// 初始化一个以Random生成随机数的随机地址选择器。
        /// </summary>
        public RandomAddressSelector()
        {
            _random = new Random();
            _generate = (min, max) => _random.Next(min, max);
        }

        /// <summary>
        /// 初始化一个自定义的随机地址选择器。
        /// </summary>
        /// <param name="generate">随机数生成委托，第一个参数为最小值，第二个参数为最大值（不可以超过该值）。</param>
        public RandomAddressSelector(Func<int, int, int> generate)
        {
            if (generate == null)
                throw new ArgumentNullException(nameof(generate));
            _generate = generate;
        }


        protected override Task<AddressModel> SelectAsync(AddressSelectorContext context)
        {
            var address = context.Address.ToArray();
            var length = address.Length;

            var index = _generate(0, length);
            return Task.FromResult(address[index]);
        }
    }
}
