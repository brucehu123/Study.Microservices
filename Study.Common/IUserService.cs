using Study.Core.Attributes;
using System.Threading.Tasks;

namespace Study.Common
{
    [RpcServiceBundle]
    public interface IUserService
    {
        Task<string> GetUserNameAsync(int id);
    }
}
