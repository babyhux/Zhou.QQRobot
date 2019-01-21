using System;
using Zhou.QQRobot.CQP.Model;

namespace Zhou.QQRobot.CQP.ApiHttpClient
{
    public static class Api
    {
        public static class QQRobotService
        {
            public static string UploadImageUrl(string baseUri) => $"{baseUri}/image/items";
            public static string CreateEventLog(string baseUri) => $"{baseUri}/GroupEvent/items/";
            public static string GetEventLog(string baseUri, GroupEventRequest request) => $"{baseUri}/GroupEvent/items/{request.FromQq}?CreatedTime={request.CreatedTime}&EventType={request.EventType}&FromQq={request.FromQq}&Source={request.Source}";
            public static string CreateMessage(string baseUri) => $"{baseUri}/GroupMessage/items/";
            public static string GetEventLogByDate(string baseUri, DateTime startEventDate, DateTime endEventDate, string eventType, string source, int pageIndex, int pageSize)
                => $"{baseUri}/GroupEvent/items/?pageindex={pageIndex}&pagesize={pageSize}&startEventDate={startEventDate.ToString("yyyy-MM-dd")}&endEventDate={endEventDate.ToString("yyyy-MM-dd")}&eventType={eventType}&source={source}";
        }
    }
}