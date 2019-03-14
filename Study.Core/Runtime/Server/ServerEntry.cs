using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.Core.Runtime.Server
{
  public  class ServerEntry
    {
        /// <summary>
        /// 服务id
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 执行委托。
        /// </summary>
        public Func<IDictionary<string, object>, Task<object>> Func { get; set; }
    }
}
