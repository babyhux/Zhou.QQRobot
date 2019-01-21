using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System.Linq;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Helper;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupHelpEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupHelpEvent(IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {

            var ats = CQNumberHelper.GetAtQQ(context.Message)?.ToList();
            if (ats != null && ats.Contains(_mahuaApi.GetLoginQq()) && (context.Message.Contains("--help") || context.Message.Contains("-h")))
            {
                _mahuaApi.SendGroupMessageExtension(context.FromGroup, RuntimeConfig.Config.HelpText);
            }
        }

    }
}
