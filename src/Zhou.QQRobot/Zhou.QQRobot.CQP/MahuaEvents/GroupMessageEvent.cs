using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using Zhou.QQRobot.CQP.ApiHttpClient;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    /// <summary>
    /// 所有群消息入库
    /// </summary>
    public class GroupMessageEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private readonly QQRobotService _qqRobotService;

        public GroupMessageEvent(
            IMahuaApi mahuaApi
            , QQRobotService qqRobotService)
        {
            _mahuaApi = mahuaApi;
            _qqRobotService = qqRobotService;
        }
        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            _qqRobotService.CreateMessage(new Model.GroupMessageRequest { FromQq = context.FromQq, MessageText = context.Message, Source = context.FromGroup, MessageDate = context.SendTime }).GetAwaiter().GetResult();
        }
    }
}
