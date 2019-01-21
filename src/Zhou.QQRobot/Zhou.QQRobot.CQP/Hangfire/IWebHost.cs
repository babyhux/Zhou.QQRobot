using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhou.QQRobot.CQP.Hangfire
{
    public interface IWebHost
    {
        /// <summary>
        /// 启动Web服务
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="lifetimeScope"></param>
        Task StartAsync(string baseUrl, ILifetimeScope lifetimeScope);

        /// <summary>
        /// 停止
        /// </summary>
        Task StopAsync();
    }
}
