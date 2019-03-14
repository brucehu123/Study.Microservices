using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Attributes
{
    /// <summary>
    /// 服务集标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceBundleAttribute : Attribute
    {
    }
}
