using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhou.QQRobot.CQP.ApiHttpClient
{
    public class ApiResult<T> : ApiResult
    {
        public T Data { get; set; }
    }
    public class ApiResult
    {
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
