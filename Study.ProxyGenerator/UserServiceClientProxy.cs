//using Study.Common;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Study.Core.Runtime.Client;

//namespace Study.ProxyGenerator
//{
//    public class UserServiceClientProxy : ServiceProxyBase, IUserService
//    {
//        public UserServiceClientProxy(IRemoteServiceInvoker remoteServiceInvoker) : base(remoteServiceInvoker)
//        {
//        }

//        public async Task<string> GetUserNameAsync(int id)
//        {
//            var parameters = new Dictionary<string, object>();
//            parameters.Add("id", id);
//            var serviceId = "Study.Common.IUserService.GetUserName_id";
//            var result = await Invoke(parameters, serviceId);
//            return result;
//        }
//    }
//}
