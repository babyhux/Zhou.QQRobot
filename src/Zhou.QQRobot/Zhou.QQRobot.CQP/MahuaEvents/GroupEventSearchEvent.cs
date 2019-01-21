using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Zhou.QQRobot.CQP.ApiHttpClient;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Helper;
using Zhou.QQRobot.CQP.Model;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    public class GroupEventSearchEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly QQRobotService _qqRobotService;
        private readonly GroupMemberInfosCache _groupMemberInfosCache;
        private const int EventLogSrachCount = 10;
        private readonly Regex regex_date = new Regex("((([0-1]{0,1}[0-9])-([0-3]{0,1}[0-9]))|今日|昨日|今天|昨天).*", RegexOptions.Compiled);
        public GroupEventSearchEvent(
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
            var ats = CQNumberHelper.GetAtQQ(context.Message)?.ToList();

            if (ats == null || !ats.Contains(_mahuaApi.GetLoginQq()))
            {
                return;
            }

            var m = regex_date.Match(content);
            if (!m.Success || m.Length == 0)
            {
                return;
            }
            DateTime date = DateTime.Now;
            string dateStr = m.Groups[1].Value;
            switch (dateStr)
            {
                case "今日":
                case "今天":
                    date = DateTime.Today;
                    break;
                case "昨日":
                case "昨天":
                    date = DateTime.Today.AddDays(-1);
                    break;
                default:
                    date = new DateTime(date.Year, int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
                    break;
            }


            foreach (var item in RuntimeConfig.Config.GroupEventSerach)
            {
                if (!context.Message.Contains(item.Key))
                    continue;
                var logs = SearchEventLog(item, date, context.FromGroup);
                if (logs != null)
                    SendMessage(logs.Data?.Items, logs.Data?.Total ?? 0, item, date, context.FromGroup);
            }
        }

        public ApiResult<PaginateItems<IEnumerable<GroupEventLog>>> SearchEventLog(GroupEventSearchConfig config, DateTime searchDate, string fromGroup)
        {
            var result = _qqRobotService.GetEventLogByDate(searchDate, searchDate.AddDays(1), config.EventType, fromGroup, 1, EventLogSrachCount).GetAwaiter().GetResult();
            return result;
        }
        public void SendMessage(IEnumerable<GroupEventLog> logs, int total, GroupEventSearchConfig config, DateTime date, string fromGroup)
        {
            var data = logs?.ToList();
            if (data?.Count() > 0)
            {
                StringBuilder content = new StringBuilder();
                foreach (var item in data)
                {
                    content.Append($"{item.CreatedTime.ToString("HH:mm")}:{_groupMemberInfosCache.GetGroupNike(fromGroup, item.CreatedBy)};");
                    if (date >= DateTime.Today.AddDays(-1))
                    {
                        content.Append($"连续{item.ContinuousCheckIn}天");
                    }
                    content.AppendLine();
                }
                var msg = config.Template
                    .Replace("{Total}", total.ToString())
                    .Replace("{Date}", date.ToString("MM-dd"))
                    .Replace("{Num}", EventLogSrachCount.ToString())
                    .Replace("{Content}", content.ToString())
                    ;
                _mahuaApi.SendGroupMessageExtension(fromGroup, msg);
            }
            else
            {
                _mahuaApi.SendGroupMessageExtension(fromGroup, config.NullTemplate);
            }
        }

    }
}
