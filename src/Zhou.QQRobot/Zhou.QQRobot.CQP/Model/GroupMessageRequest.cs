using System;

namespace Zhou.QQRobot.CQP.Model
{
    public class GroupMessageRequest
    {
        public string MessageText { get; set; }
        public string Source { get; set; }
        public string FromQq { get; set; }
        public DateTime MessageDate { get; set; }
    }
}
