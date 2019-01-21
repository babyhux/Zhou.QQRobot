using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Extension;
using Zhou.QQRobot.CQP.Helper;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupManageEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private static readonly Regex banRegex = new Regex("禁言.+时间:(\\d+)分钟", RegexOptions.Compiled);
        private static readonly Regex repeatBanTimeChangeRegex = new Regex("复读禁言时间变更:(\\d+)分钟", RegexOptions.Compiled);
        private static readonly Regex repeatBanProbabilityChangeRegex = new Regex("复读禁言概率变更为(\\d+)%", RegexOptions.Compiled);

        public GroupManageEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            var managerQQ = RuntimeConfig.Config.ManagerQq.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var message = context.Message;
            var fromQQ = context.FromQq;
            var group = context.FromGroup;
            var loginQQ = _mahuaApi.GetLoginQq();
            if (managerQQ.Any(x => x == fromQQ))
            {
                var ats = CQNumberHelper.GetAtQQ(context.Message)?.ToList();
                if (!ats?.Any(x => x == loginQQ) ?? false)
                {
                    return;
                }
                //解除禁言
                if (message.IndexOf("解除禁言") > -1)
                {
                    foreach (var item in ats.Where(x => x != loginQQ))
                    {
                        _mahuaApi.RemoveBanGroupMember(group, item);
                    }
                }
                else
                {
                    var m = banRegex.Match(message);
                    //禁言
                    if (m.Success)
                    {
                        var time = Convert.ToDouble(m.Groups[1].Value);
                        foreach (var item in ats?.Where(x => x != loginQQ))
                        {
                            _mahuaApi.BanGroupMember(context.FromGroup, item, TimeSpan.FromMinutes(time));
                        }
                        return;
                    }
                    //修改复读禁言时间
                    m = repeatBanTimeChangeRegex.Match(message);
                    if (m.Success)
                    {
                        var time = Convert.ToDouble(m.Groups[1].Value);
                        if (time > 0)
                        {
                            RuntimeConfig.Config.RepeatMessageBanDuration = time;
                            RuntimeConfig.Save(AppSetting.ConfigFilePath, RuntimeConfig.Config.ToJosn());
                        }
                        return;
                    }
                    //修改复读禁言概率
                    m = repeatBanProbabilityChangeRegex.Match(message);
                    if (m.Success)
                    {
                        var time = Convert.ToDouble(m.Groups[1].Value);
                        if (time > 0)
                        {
                            RuntimeConfig.Config.RepeatMessageBanProbability = time;
                            RuntimeConfig.Save(AppSetting.ConfigFilePath, RuntimeConfig.Config.ToJosn());
                        }
                        return;
                    }
                }
            }
        }
    }
}
