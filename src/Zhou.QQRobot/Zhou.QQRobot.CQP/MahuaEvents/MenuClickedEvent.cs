using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;
using Zhou.QQRobot.CQP.Config;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    /// <summary>
    /// 菜单点击事件
    /// </summary>
    public class MenuClickedEvent
        : IMahuaMenuClickedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public MenuClickedEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessManhuaMenuClicked(MahuaMenuClickedContext context)
        {
        }
    }
}
