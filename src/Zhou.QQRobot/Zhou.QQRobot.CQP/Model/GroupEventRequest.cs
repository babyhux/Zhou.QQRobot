using System;

namespace Zhou.QQRobot.CQP.Model
{
    public class GroupEventRequest
    {
        public string EventText { get; set; }
        public string EventType { get; set; }

        public string FromQq { get; set; }
        public string Source { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
