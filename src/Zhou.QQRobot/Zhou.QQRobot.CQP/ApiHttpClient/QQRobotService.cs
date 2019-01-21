using Newbe.Mahua.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Zhou.QQRobot.CQP.Config;
using Zhou.QQRobot.CQP.Extension;
using Zhou.QQRobot.CQP.Model;

namespace Zhou.QQRobot.CQP.ApiHttpClient
{
    public sealed class QQRobotService
    {
        private readonly HttpClient _client;

        public QQRobotService(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiResult<string>> UploadImage(List<(string name, byte[] bytes)> images, string fromQQ)
        {
            if (images == null || images.Count == 0 || string.IsNullOrEmpty(fromQQ))
            {
                return default;
            }
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Api.QQRobotService.UploadImageUrl(AppSetting.QQRobotUrl));
            using (var content = new MultipartFormDataContent())
            {
                var fromqq = new StringContent(fromQQ);
                fromqq.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "fromQQ" };
                content.Add(fromqq);
                foreach (var (name, bytes) in images)
                {
                    var fileContent = new ByteArrayContent(bytes);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        FileName = name,
                        Name = "FormFiles"
                    };
                    content.Add(fileContent);
                }

                httpRequestMessage.Content = content;
                try
                {
                    var response = await _client.SendAsync(httpRequestMessage);
                    using (var responseContent = response.Content)
                    {
                        var result = await responseContent.ReadAsStringAsync();
                        return result.FromJson<ApiResult<string>>();
                    }
                }
                catch (Exception e)
                {
                    var logger = LogProvider.For<QQRobotService>();
                    logger.Error(e.ToString());
                    return default;
                }
            }
        }

        public async Task<ApiResult<GroupEventLog>> CreateEventLog(GroupEventRequest @event)
        {
            var uri = Api.QQRobotService.CreateEventLog(AppSetting.QQRobotUrl);

            try
            {
                var response = await _client.PostAsync(uri, new StringContent(@event.ToJosn(), Encoding.UTF8, "application/json"));
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    return result.FromJson<ApiResult<GroupEventLog>>();
                }
            }
            catch (Exception e)
            {
                var logger = LogProvider.For<QQRobotService>();
                logger.Error(e.ToString());
                return default;
            }
        }
        public async Task<ApiResult<GroupEventLog>> GetEventLog(GroupEventRequest @event)
        {
            var uri = Api.QQRobotService.GetEventLog(AppSetting.QQRobotUrl, @event);

            try
            {
                var response = await _client.GetStringAsync(uri);
                return response.FromJson<ApiResult<GroupEventLog>>();
            }
            catch (Exception e)
            {
                var logger = LogProvider.For<QQRobotService>();
                logger.Error(e.ToString());
                return default;
            }
        }
        public async Task<ApiResult<PaginateItems<IEnumerable<GroupEventLog>>>> GetEventLogByDate(DateTime startKotowDate, DateTime endKotowDate, string type, string source, int pageIndex, int pageSize)
        {
            var uri = Api.QQRobotService.GetEventLogByDate(AppSetting.QQRobotUrl, startKotowDate, endKotowDate, type, source, pageIndex, pageSize);
            try
            {
                var response = await _client.GetStringAsync(uri);
                return response.FromJson<ApiResult<PaginateItems<IEnumerable<GroupEventLog>>>>();
            }
            catch (Exception e)
            {
                var logger = LogProvider.For<QQRobotService>();
                logger.Error(e.ToString());
                return default;
            }
        }
        public async Task<ApiResult<GroupEventLog>> CreateMessage(GroupMessageRequest request)
        {
            var uri = Api.QQRobotService.CreateMessage(AppSetting.QQRobotUrl);

            try
            {
                var response = await _client.PostAsync(uri, new StringContent(request.ToJosn(), Encoding.UTF8, "application/json"));
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    return result.FromJson<ApiResult<GroupEventLog>>();
                }
            }
            catch (Exception e)
            {
                var logger = LogProvider.For<QQRobotService>();
                logger.Error(e.ToString());
                return default;
            }
        }

    }
}
