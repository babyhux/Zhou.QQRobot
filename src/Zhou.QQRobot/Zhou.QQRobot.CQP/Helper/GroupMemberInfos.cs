using Newbe.Mahua;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zhou.QQRobot.CQP.Config;

namespace Zhou.QQRobot.CQP.Helper
{
    public class GroupMemberInfosCache
    {

        private readonly ConcurrentDictionary<string, List<GroupMemberInfo>> _groupMemberInfoCache = new ConcurrentDictionary<string, List<GroupMemberInfo>>();

        public List<GroupMemberInfo> GetGroupMemberInfo(string groupNumber)
        {
            return _groupMemberInfoCache.GetOrAdd(groupNumber, x =>
            {
                using (var session = MahuaRobotManager.Instance.CreateSession())
                    return session.MahuaApi.GetGroupMemebersWithModel(x).Model.ToList();
            });
        }

        public string GetGroupNike(string fromGroup, string fromqq)
        {
            var fromQQInfo = GetGroupMemberInfo(fromGroup)?.FirstOrDefault(x => x.Qq == fromqq);
            var fromQQGroupName = fromQQInfo?.InGroupName;
            if (string.IsNullOrWhiteSpace(fromQQGroupName)) fromQQGroupName = fromQQInfo?.NickName;
            if (string.IsNullOrWhiteSpace(fromQQGroupName)) fromQQGroupName = fromqq;
            return fromQQGroupName;
        }
    }
}
