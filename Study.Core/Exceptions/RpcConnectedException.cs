using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Exceptions
{
    /// <summary>
    /// 表示远程连接时发生异常
    /// </summary>
    public class RpcConnectedException : RpcException
    {
        public RpcConnectedException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
