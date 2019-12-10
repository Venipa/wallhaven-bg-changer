using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallhaven_BG_Changer.API;

namespace Wallhaven_BG_Changer.Tests
{
    [TestFixture]
    class IntervalChangeTest
    {
        [TestCase]
        public void test_interval()
        {
            var wh = new WallhavenClient();
            var item = wh.search("Sasuke -Yellow", "random", "1M", true).Result?.data?.FirstOrDefault();
            if (item != null)
            {
                var data = wh.downloadWallpaper(item).Result;
                if (data != null)
                {
                    var endPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(item.path));
                    File.WriteAllBytes(endPath, data);
                    App.DisplayPicture(endPath);
                }
            }
        }
    }
}
