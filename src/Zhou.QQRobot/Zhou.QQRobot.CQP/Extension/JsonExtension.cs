using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhou.QQRobot.CQP.Extension
{
    public static class JsonExtension
    {
        /// <summary>
        /// 序列化json对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJosn(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 反序列化json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
