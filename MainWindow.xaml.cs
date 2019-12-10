using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Wallhaven_BG_Changer.API;

namespace Wallhaven_BG_Changer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WallhavenBGCApp : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool isOnStartup { get; set; }
        [AlsoNotifyFor("isTopSort")]
        public string SelectedSort { get; set; }
        public string SearchQuery { get; set; }
        public uint intervalTimeChange { get; set; }
        public string SelectedTopRange { get; set; }
        public bool isTopSort { get => this.SelectedSort == "toplist";  }

        public List<string> sortTypes { get; set; } = new List<string>();
        public List<string> topRanges { get; set; } = new List<string>();
        public WallhavenBGCApp()
        {
            InitializeComponent();
            this.sortTypes = "date_added,relevance,random,views,favorites,toplist".Split(',').ToList();
            this.topRanges = "1d,3d,1w,1M,3M,6M,1y".Split(',').ToList();
            this.SelectedSort = (string)Application.Current.Properties["selectedSort"] ?? this.sortTypes.Last();
            this.SelectedTopRange = (string)Application.Current.Properties["selectedTopRange"] ?? this.topRanges.Skip(3).First();
            this.isOnStartup = App.Config.isOnStartup;
            this.SearchQuery = App.Config.SearchQuery;
            this.intervalTimeChange = App.Config.ChangeInterval;
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            //Perform property validation
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            if (propertyName == "isOnStartup")
            {
                this.setStartup((bool)after);
                App.Config.isOnStartup = (bool)after;
            }
            if (propertyName == "SearchQuery")
            {
                App.Config.SearchQuery = (string)after;
            }
            if (propertyName == "SelectedTopRange" && this.topRanges.IndexOf((string)after) > -1)
            {
                App.Config.selectedTopRange = (string)after;
            }
            if (propertyName == "SelectedSort" && this.sortTypes.IndexOf((string)after) > -1)
            {
                App.Config.selectedSort = (string)after;
                this.OnPropertyChanged("isTopSort", !this.isOnStartup, this.isOnStartup);
            }
            if (propertyName == "intervalTimeChange" && (uint)after > 10000)
            {
                App.Config.ChangeInterval = (uint)after;
            }
        }

        void setStartup(bool enabled)
        {
            var exec = Assembly.GetExecutingAssembly();
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (enabled)
                rk.SetValue(exec.FullName, string.Join("", exec.Location.Concat(" -s")));
            else
                rk.DeleteValue(exec.FullName, false);
        }

        private void onIntervalValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void updateBackground()
        {

            var wh = new WallhavenClient();
            var item = wh.search(this.SearchQuery, this.SelectedSort, this.SelectedTopRange, true).Result?.data?.FirstOrDefault();
            if (item != null)
            {
                var wpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wallpapers");
                if (!Directory.Exists(wpPath))
                {
                    Directory.CreateDirectory(wpPath);
                }
                var data = wh.downloadWallpaper(item).Result;
                if (data != null)
                {
                    var endPath = Path.Combine(wpPath, Path.GetFileName(item.path));
                    File.WriteAllBytes(endPath, data);
                    App.DisplayPicture(endPath);
                }
            }
        }

        private void onUpdateWallpaper(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                this.updateBackground();
            }));
        }
    }
}
