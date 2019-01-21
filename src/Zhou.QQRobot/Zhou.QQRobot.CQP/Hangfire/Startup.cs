using Autofac;
using Hangfire;
using Hangfire.MemoryStorage;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Zhou.QQRobot.CQP.Hangfire.Startup))]
namespace Zhou.QQRobot.CQP.Hangfire
{
    public class Startup
    {
        public void Configuration(IAppBuilder app, ILifetimeScope lifetimeScope)
        {
            // 初始化Hangfire
            var config = GlobalConfiguration.Configuration;

            // 使用内存存储任务，若有持久化任务的需求，可以根据Hangfire的文档使用数据库方式存储
            config.UseMemoryStorage();

            // 通过Autofac容器来实现任务的构建
            config.UseAutofacActivator(lifetimeScope);

            // 启用Hangfire的web界面
            app.UseHangfireDashboard();

            // 初始化Hangfire服务
            app.UseHangfireServer();

        }
    }
}
