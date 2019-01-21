using Newbe.Mahua;
using Newbe.Mahua.Logging;
using System;
using System.Collections.Concurrent;
using Zhou.QQRobot.CQP.Config;

namespace Zhou.QQRobot.CQP.Helper
{
    public static class GroupApi
    {
        private static ConcurrentDictionary<string, DateTime> lastSendMessageTime = new ConcurrentDictionary<string, DateTime>();
        private static ConcurrentDictionary<string, object> sendMessageTimeLock = new ConcurrentDictionary<string, object>();
        private static readonly ILog logger = LogProvider.GetCurrentClassLogger();

        public static void SendGroupMessageExtension(this IMahuaApi api, string toGroup, string message)
        {
            var now = DateTime.Now;
            var lastTime = lastSendMessageTime.GetOrAdd(toGroup, now.AddDays(-1));
            if (lastTime.AddSeconds(RuntimeConfig.Config.SendMessageInterval) < now)
            {
                var objectLock = sendMessageTimeLock.GetOrAdd(toGroup, new object());
                lock (objectLock)
                {
                    if (lastSendMessageTime.TryGetValue(toGroup, out DateTime sendMessageTime) && sendMessageTime.AddSeconds(RuntimeConfig.Config.SendMessageInterval) < now)
                    {
                        api.SendGroupMessage(toGroup, message);
                        lastSendMessageTime.TryUpdate(toGroup, now, sendMessageTime);
                    }
                }
            }
            else
            {
                logger.Info($"当前时间:{now},上一次发言时间:{lastTime},群号:{toGroup}");
            }
        }
    }
}
