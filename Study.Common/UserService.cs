using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.Common
{
    public class UserService : IUserService
    {
        public Task<string> GetUserNameAsync(int id)
        {
            return Task.FromResult<string>($"id:{id} 的名称时小小胡");
        }
    }
}
