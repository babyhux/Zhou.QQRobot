using Autofac;
using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System.IO;
using System.Net.Http;
using System.Text;
using Zhou.QQRobot.CQP.ApiHttpClient;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Extension;
using Zhou.QQRobot.CQP.Hangfire;
using Zhou.QQRobot.CQP.Helper;
using Zhou.QQRobot.CQP.MahuaEvents;

namespace Zhou.QQRobot.CQP
{
    /// <summary>
    /// Ioc容器注册
    /// </summary>
    public class MahuaModule : IMahuaModule
    {
        public Module[] GetModules()
        {
            // 可以按照功能模块进行划分，此处可以改造为基于文件配置进行构造。实现模块化编程。
            return new Module[]
            {
                new PluginModule(),
                new MahuaEventsModule(),
                new MyServiceModule(),
            };
        }

        /// <summary>
        /// 基本模块
        /// </summary>
        private class PluginModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                // 将实现类与接口的关系注入到Autofac的Ioc容器中。如果此处缺少注册将无法启动插件。
                // 注意！！！PluginInfo是插件运行必须注册的，其他内容则不是必要的！！！
                builder.RegisterType<PluginInfo>()
                    .As<IPluginInfo>();

                //注册在“设置中心”中注册菜单，若想订阅菜单点击事件，可以查看教程。http://www.newbe.pro/docs/mahua/2017/12/24/Newbe-Mahua-Navigations.html
                builder.RegisterType<MyMenuProvider>()
                    .As<IMahuaMenuProvider>();

            }
        }

        /// <summary>
        /// <see cref="IMahuaEvent"/> 事件处理模块
        /// </summary>
        private class MahuaEventsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                // 将需要监听的事件注册，若缺少此注册，则不会调用相关的实现类
                builder.RegisterType<InitializationMahuaEvent>().As<IInitializationMahuaEvent>();
                builder.RegisterType<GroupImageUploadEvent>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<GroupEventLogEvent>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<GroupEventSearchEvent>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<GroupMessageEvent>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<GroupHelpEvent>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<GroupRepeatEvent>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<GroupManageEvent>().As<IGroupMessageReceivedMahuaEvent>();
                builder.RegisterType<MenuClickedEvent>().As<IMahuaMenuClickedMahuaEvent>();
            }

        }
        private class MyServiceModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);


                builder.RegisterType<HttpClient>().SingleInstance();
                builder.RegisterType<QQRobotService>().SingleInstance();
                builder.RegisterType<GroupMemberInfosCache>().SingleInstance();

                builder.RegisterType<RecurringJobs>().SingleInstance();

                // 确保Web服务是单例
                builder.RegisterType<OwinWebHost>()
                    .As<IWebHost>()
                    .SingleInstance();
                RuntimeConfig.Config = File.ReadAllText(AppSetting.ConfigFilePath, Encoding.UTF8).FromJson<BizConfig>();
            }
        }
    }
}
