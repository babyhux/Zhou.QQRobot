using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Zhou.QQRobot.CQP.Config
{
    public static class AppSetting
    {

        public static string GroupNumber => ConfigurationManager.AppSettings["GroupNumber"];

        public static string QQRobotUrl => ConfigurationManager.AppSettings["QQRobotUrl"];

        public static string SignInNotifyQQ => ConfigurationManager.AppSettings["SignInNotifyQQ"];

        public static string ConfigFilePath
        {
            get
            {
                var pluginApiExpDll = typeof(MahuaModule).Assembly.CodeBase;
                var pluginName = Path.GetFileNameWithoutExtension(pluginApiExpDll);
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var dllDir = Path.GetFullPath(Path.Combine(baseDir, pluginName));
                var path = ConfigurationManager.AppSettings["ConfigFilePath"];

                return Path.Combine(dllDir, path);
            }
        }
    }


    public static class RuntimeConfig
    {
        public static BizConfig Config { get; set; }
        private static readonly object WriteLock = new object();
        public static void Save(string path, string content)
        {
            var pluginApiExpDll = typeof(MahuaModule).Assembly.CodeBase;
            var pluginName = Path.GetFileNameWithoutExtension(pluginApiExpDll);
            lock (WriteLock)
            {
                File.WriteAllText(path, content, Encoding.UTF8);
            }
        }
    }

    public class GroupEventConfig
    {
        private Regex regex = null;
        public string Rule { get; set; }

        [JsonIgnore]
        public Regex Regex
        {
            get
            {
                if (regex == null)
                {
                    regex = new Regex(Rule, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
                return regex;
            }
        }
        public string QQ { get; set; }
        public string DefaultReply { get; set; }
        public string EventReply { get; set; }
        public string EventType { get; set; }
    }
    public class GroupEventSearchConfig
    {
        public string Key { get; set; }
        public string EventType { get; set; }
        public string Template { get; set; }
        public string NullTemplate { get; set; }
    }

    public class BizConfig
    {
        //复读队列禁言时间
        public double RepeatMessageBanDuration { get; set; }

        //复读队列禁言概率
        public double RepeatMessageBanProbability { get; set; }

        //发送消息间隔
        public double SendMessageInterval { get; set; }

        //是否跟随复读
        public bool IsRepeatMessage { get; set; }

        //是否禁言复读
        public bool IsBanRepeatMessage { get; set; }

        //指令帮助
        public string HelpText { get; set; }

        //群事件
        public IList<GroupEventConfig> GroupEvent { get; set; }

        public string ManagerQq { get; set; }

        //群事件搜索
        public IList<GroupEventSearchConfig> GroupEventSerach { get; set; }

    }
}
