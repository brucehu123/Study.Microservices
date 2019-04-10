using Consul;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.Consul.Utilitys
{
    public static class ConsulClientExtensions
    {
        public static async Task<string[]> GetChildrenAsync(this ConsulClient _consul, string path)
        {
            try
            {
                var queryResut = await _consul.KV.List(path);
                return
                    queryResut.Response?.Select(s => Encoding.UTF8.GetString(s.Value)).ToArray();
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public static async Task<byte[]> GetDataAsync(this ConsulClient _consul, string path)
        {
            try
            {
                var queryResut = await _consul.KV.Get(path);
                return queryResut.Response?.Value;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
