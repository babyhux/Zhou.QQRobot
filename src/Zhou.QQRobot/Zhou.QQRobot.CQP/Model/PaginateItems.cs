using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhou.QQRobot.CQP.Model
{
    public class PaginateItems<T>
    {
        public T Items { get; set; }
        public int Total { get; set; }
    }
}
