using System;
using static Zhou.QQRobot.CQP.ApiHttpClient.Api.QQRobotService;

namespace Zhou.QQRobot.CQP.Model
{
    /// <summary>
    /// 磕头日志
    /// </summary>
    public class GroupEventLog : BaseEntity<long, string>
    {
        public int ContinuousCheckIn { get; set; }
        public string EventText { get; set; }

        public string EventType { get; set; }

        public string Source { get; set; }
    }

    public abstract class BaseEntity<TKey, TUserKey>
    {
        public virtual TKey Id { get; set; }

        public TUserKey CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public TUserKey ModifiedBy { get; set; }

        public DateTime ModifiedTime { get; set; }
        public bool IsDeleted { get; set; }
    }


}
