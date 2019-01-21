using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;
using Zhou.QQRobot.CQP.ApiHttpClient;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Helper;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupEventLogEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly QQRobotService _qqRobotService;
        private readonly GroupMemberInfosCache _groupMemberInfosCache;

        public GroupEventLogEvent(
            IMahuaApi mahuaApi
            , QQRobotService qqRobotService
            , GroupMemberInfosCache groupMemberInfosCache)
        {
            _mahuaApi = mahuaApi;
            _qqRobotService = qqRobotService;
            _groupMemberInfosCache = groupMemberInfosCache;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            var content = context.Message;
            foreach (var item in RuntimeConfig.Config.GroupEvent)
            {
                if (item.Regex.IsMatch(content))
                {
                    if (context.FromQq == item.QQ)
                    {
                        _mahuaApi.SendGroupMessageExtension(context.FromGroup, item.DefaultReply);
                        continue;
                    }
                    var request = new Model.GroupEventRequest { FromQq = context.FromQq, EventText = context.Message, EventType = item.EventType, Source = context.FromGroup, CreatedTime = DateTime.Now };
                    //疲劳值, 防止过多重复磕头刷屏. 只回复第一次有效数据
                    var kotowLog = _qqRobotService.GetEventLog(request).GetAwaiter().GetResult()?.Data;
                    if (kotowLog != null)
                    {
                        continue;
                    }
                    kotowLog = _qqRobotService.CreateEventLog(request).GetAwaiter().GetResult()?.Data;
                    if (!string.IsNullOrWhiteSpace(item.EventReply))
                    {
                        var msg = item.EventReply
                            .Replace("{Name}", _groupMemberInfosCache.GetGroupNike(context.FromGroup, context.FromQq))
                            .Replace("{IsContinuous}", (kotowLog.ContinuousCheckIn > 1 ? "连续" : ""))
                            .Replace("{ContinuousNum}", kotowLog.ContinuousCheckIn.ToString());
                        _mahuaApi.SendGroupMessageExtension(context.FromGroup, msg);
                    }
                }
            }
        }
    }
}
