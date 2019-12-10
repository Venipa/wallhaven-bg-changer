using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Wallhaven_BG_Changer.API;

namespace Wallhaven_BG_Changer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppConfig Config;
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args?.Length > 0)
            {
                if (e.Args.Contains("-s"))
                {
                    this.startChangeProcess();
                }
                return;
            }
            base.OnStartup(e);
            Config = new AppConfig();
            Config.Load().Wait();
            Application.Current.Exit += (s, ev) =>
            {
                Config.Save().Wait();
            };
        }
        [STAThread]
        public void startChangeProcess()
        {
            Config = new AppConfig();
            Config.Load().Wait();
            var wh = new WallhavenClient();
            var bgInterval = App.Config.ChangeInterval;
            var changeOnStart = App.Config.ChangeOnStart;
            var bgChanger = new Timer(bgInterval);
            var wpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wallpapers");
            bgChanger.Elapsed += (s, e) =>
            {
                try
                {
                    bgChanger.Stop();
                    var item = wh.search(Config.SearchQuery, Config.selectedSort, Config.selectedTopRange, true).Result?.data?.FirstOrDefault();
                    if (item != null)
                    {
                        var data = wh.downloadWallpaper(item).Result;
                        if (data != null)
                        {
                            if (!Directory.Exists(wpPath))
                            {
                                Directory.CreateDirectory(wpPath);
                            }
                            var endPath = Path.Combine(wpPath, Path.GetFileName(item.path));
                            File.WriteAllBytes(endPath, data);
                            if (File.Exists(endPath))
                            {
                                DisplayPicture(endPath);
                            }
                            bgChanger.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log"), ex.ToString());
                    bgChanger.Start();
                }
            };
            bgChanger.Start();
            Task.Delay(-1).Wait();
        }
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x1;
        private const uint SPIF_SENDWININICHANGE = 0x2;

        public static void DisplayPicture(string file_name)
        {
            uint flags = 0;
            if (!SystemParametersInfo(SPI_SETDESKWALLPAPER,
                    0, file_name, flags))
            {
                Console.WriteLine("Error");
            }
        }
    }
}
