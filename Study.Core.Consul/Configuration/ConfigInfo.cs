using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Consul.Configuration
{
    public class ConfigInfo
    {
        /// <summary>
        /// watch 时间间隔
        /// </summary>
        public int WatchInterval { get; set; } = 60;
        /// <summary>
        /// 是否启用子节点监控
        /// </summary>
        public bool EnableChildrenMonitor { get; set; } = true;
        /// <summary>
        /// 路由配置路径。
        /// </summary>
        public string RoutePath { get; set; }
        /// <summary>
        /// host地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 会话超时时间。
        /// </summary>
        public TimeSpan SessionTimeout { get; set; }
    }
}
