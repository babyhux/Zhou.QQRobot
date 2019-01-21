using Newbe.Mahua;
using System;
using System.Linq;
using System.Text;
using Zhou.QQRobot.CQP.ApiHttpClient;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Helper;

namespace Zhou.QQRobot.CQP.Hangfire
{
    public class RecurringJobs
    {
        private readonly QQRobotService _qqRobotService;
        private readonly GroupMemberInfosCache _groupMemberInfosCache;

        public RecurringJobs(QQRobotService qqRobotService, GroupMemberInfosCache groupMemberInfosCache)
        {
            _qqRobotService = qqRobotService;
            _groupMemberInfosCache = groupMemberInfosCache;
        }

        public void KotowNotify()
        {
            const int kotowCount = 10;
            var groupNumber = AppSetting.GroupNumber;
            var result = _qqRobotService.GetEventLogByDate(DateTime.Now.AddDays(-1), DateTime.Now, "Kowtow", groupNumber, 1, kotowCount).GetAwaiter().GetResult();
            var kotows = result?.Data.Items;

            using (var robotSession = MahuaRobotManager.Instance.CreateSession())
            {
                var api = robotSession.MahuaApi;
                if (kotows?.Count() > 0)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append($"磕头播报: 昨日共磕头{result?.Data.Total}次,前{kotowCount}位磕头者:");
                    foreach (var item in kotows)
                    {
                        msg.AppendLine();
                        msg.Append($"{item.CreatedTime.ToString("HH:mm")}:{_groupMemberInfosCache.GetGroupNike(groupNumber, item.CreatedBy)};已连续磕头{item.ContinuousCheckIn}天");
                    }
                    api.SendGroupMessage(groupNumber, msg.ToString());
                }
                else
                {
                    api.SendGroupMessage(groupNumber).Text("一个时代一个神,昨天没人磕头,教主牛逼.").Done();
                }
            }
        }
        public void SignInNotify()
        {
            var qq = AppSetting.SignInNotifyQQ.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (qq.Length == 0) return;
            for (int i = 0; i < qq.Length; i++)
            {
                using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                {
                    var api = robotSession.MahuaApi;
                    api.SendPrivateMessage(qq[i]).Text("签到拉").Done();
                }

            }
        }
    }
}
