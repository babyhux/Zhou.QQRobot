using Newbe.Mahua;
using System.Collections.Generic;

namespace Zhou.QQRobot.CQP
{
    public class MyMenuProvider : IMahuaMenuProvider
    {
        public IEnumerable<MahuaMenu> GetMenus()
        {
            return new[]
            {
                new MahuaMenu
                {
                    Id = "menu1",
                    Text = "menu1"
                },
            };
        }
    }
}
