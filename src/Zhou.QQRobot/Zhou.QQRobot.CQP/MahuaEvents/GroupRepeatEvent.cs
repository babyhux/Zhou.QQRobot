using Newbe.Mahua;
using Newbe.Mahua.Logging;
using Newbe.Mahua.MahuaEvents;
using System;
using System.Collections.Concurrent;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Helper;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    public class GroupRepeatEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        private static readonly Random random = new Random();
        private static readonly object randLock = new object();
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<string>> messageCache = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> messageCacheCount = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>();
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> repeatMessageCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
        //private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> banMessageCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
        private const int MessageCacheCount = 30;
        private static readonly object ClearMessageCacheLock = new object();
        private ILog logger = LogProvider.For<GroupRepeatEvent>();


        public GroupRepeatEvent(IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            
            var group = context.FromGroup;
            var message = context.Message;
            var currentGroupMessageCache = messageCache.GetOrAdd(group, new ConcurrentQueue<string>());
            //当前缓存消息出现的次数
            var currentGroupMessageCacheCount = messageCacheCount.GetOrAdd(group, new ConcurrentDictionary<string, byte>());
            //已复读
            var currentGroupRepeatMessageCache = repeatMessageCache.GetOrAdd(group, new ConcurrentDictionary<string, bool>());
            //已禁言
            //var currentGroupBanMessageCache = banMessageCache.GetOrAdd(group, new ConcurrentDictionary<string, bool>());
            while (currentGroupMessageCache.Count >= MessageCacheCount)
            {
                lock (ClearMessageCacheLock)
                {

                    if (currentGroupMessageCache.Count >= MessageCacheCount
                        && currentGroupMessageCache.TryDequeue(out string oldMessage))
                    {

                        //确保队列缓存被处理了
                        byte oldCount = 0;
                        while (currentGroupMessageCacheCount.TryGetValue(oldMessage, out oldCount))
                        {
                            if (currentGroupMessageCacheCount.TryUpdate(oldMessage, (byte)(oldCount - 1), oldCount))
                            {
                                break;
                            }
                        }
                        if (oldMessage != message && oldCount <= 1)
                        {
                            currentGroupMessageCacheCount.TryRemove(oldMessage, out _);
                            currentGroupRepeatMessageCache.TryRemove(oldMessage, out _);
                            break;
                        }
                    }
                }
            }


            currentGroupMessageCache.Enqueue(message);
            var messageCount = currentGroupMessageCacheCount.AddOrUpdate(message, 1, (_, count) => (byte)(count + 1));


            //3条起 20%增长, 60%封顶
            var maxProbality = (100 - (messageCount - 2 >= 3 ? 3 : messageCount - 2) * 20);
            var probability = 0;
            lock (randLock)
            {
                probability = random.Next(1, 101);
            }
            if (messageCount > 2)
            {
                //复读
                if (RuntimeConfig.Config.IsRepeatMessage && probability > maxProbality && !currentGroupRepeatMessageCache.ContainsKey(message))
                {
                    currentGroupRepeatMessageCache.TryAdd(message, true);
                    _mahuaApi.SendGroupMessageExtension(group, message);
                }
                //禁言
                var p = 100 - RuntimeConfig.Config.RepeatMessageBanProbability;
                if (RuntimeConfig.Config.IsBanRepeatMessage && probability > p)
                {
                    _mahuaApi.BanGroupMember(group, context.FromQq, TimeSpan.FromMinutes(RuntimeConfig.Config.RepeatMessageBanDuration));
                }
            }
        }
    }
}
