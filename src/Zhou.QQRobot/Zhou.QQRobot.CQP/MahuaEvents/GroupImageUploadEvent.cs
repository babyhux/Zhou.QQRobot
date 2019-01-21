using Newbe.Mahua;
using Newbe.Mahua.Logging;
using Newbe.Mahua.MahuaEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zhou.QQRobot.CQP.ApiHttpClient;
using Zhou.QQRobot.CQP.Helper;

namespace Zhou.QQRobot.CQP.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupImageUploadEvent
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly Regex regex_URL = new Regex(@"https?:\/\/(([a-zA-Z0-9_-])+(\.)?)*(:\d+)?(\/((\.)?(\?)?=?&?[a-zA-Z0-9_-](\?)?)*)*", RegexOptions.IgnoreCase);
        private readonly IMahuaApi _mahuaApi;
        private readonly QQRobotService _qqRobotService;
        private readonly GroupMemberInfosCache _groupMemberInfosCache;
        private readonly HttpClient _client;

        public GroupImageUploadEvent(
            IMahuaApi mahuaApi, QQRobotService qqRobotService, GroupMemberInfosCache groupMemberInfosCache, HttpClient client)
        {
            _mahuaApi = mahuaApi;
            _qqRobotService = qqRobotService;
            _groupMemberInfosCache = groupMemberInfosCache;
            _client = client;
        }


        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            var ats = CQNumberHelper.GetAtQQ(context.Message)?.ToList();

            if (ats != null && ats.Contains(_mahuaApi.GetLoginQq()))
            {
                if (context.Message.Contains("上传图片"))
                {
                    var result = UploadImageAsync(context).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(result))
                    {
                        _mahuaApi.SendGroupMessageExtension(context.FromGroup, result);
                        ILog logger = LogProvider.GetCurrentClassLogger();
                        logger.Info(result);
                    }
                }
            }
        }

        private async Task<string> UploadImageAsync(GroupMessageReceivedContext context)
        {
            var imagesFileNames = CQNumberHelper.GetImage(context.Message)?.ToList();
            if (imagesFileNames == null)
            {
                return "没图片at我搞毛啊.";
            }
            List<(string name, byte[] bytes)> images = new List<(string name, byte[] bytes)>();
            foreach (var fileName in imagesFileNames)
            {
                string imgUrl = await GetImageUrlAsync(fileName);
                if (string.IsNullOrEmpty(imgUrl)) continue;

                var imgBytes = await GetByteArrayAsync(imgUrl);
                if (imgBytes.Length == 0) continue;

                images.Add((fileName, imgBytes));
            }

            var result = await _qqRobotService.UploadImage(images, context.FromQq);
            return $"{_groupMemberInfosCache.GetGroupNike(context.FromGroup, context.FromQq)}的图片:{result.Message}";
        }

        private async Task<string> GetImageUrlAsync(string fileName)
        {
            using (var sr = File.OpenText(Path.Combine(Environment.CurrentDirectory, "data", "image", fileName + ".cqimg")))
            {
                var text = await sr.ReadToEndAsync();
                return regex_URL.Match(text).Value;
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            return await _client.GetByteArrayAsync(url);
        }

    }
}

