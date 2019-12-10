using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wallhaven_BG_Changer
{
    public class AppConfig
    {
        [JsonIgnore]
        public string BaseDirectory { get => AppDomain.CurrentDomain.BaseDirectory; }
        [JsonIgnore]
        public string ConfigPath { get => Path.Combine(this.BaseDirectory, "settings.json"); }
        [JsonProperty("selected_sort")]
        public string selectedSort { get; set; }
        [JsonProperty("selected_top_range")]
        public string selectedTopRange { get; set; }
        [JsonProperty("run_on_startup")]
        public bool isOnStartup { get; set; } = false;
        [JsonProperty("search_query")]
        public string SearchQuery { get; set; } = "";
        [JsonProperty("change_interval")]
        public uint ChangeInterval { get; set; } = 60 * 1000 * 60 * 4;
        [JsonProperty("change_upon_start")]
        public bool ChangeOnStart { get; set; } = true;


        public async Task Save()
        {
            var data = JsonConvert.SerializeObject(this);
            File.WriteAllText(this.ConfigPath, data);
        }
        public async Task Load()
        {
            if (File.Exists(this.ConfigPath))
            {
                var data = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(this.ConfigPath));
                if (data != null)
                {
                    var thisType = this.GetType();
                    data.GetType().GetProperties().Where(x => x.GetCustomAttribute<JsonPropertyAttribute>() != null && x.CanWrite).ToList().ForEach(x =>
                    {
                        var p = thisType.GetProperty(x.Name);
                        if (p?.CanWrite == true)
                        {
                            p?.SetValue(this, x.GetValue(data));
                        }
                    });
                }
            }
        }
    }
    public class AppConfigParamAttribute : Attribute
    {
        public string serializedName { get; private set; }
        public AppConfigParamAttribute(string serializedName)
        {
            this.serializedName = serializedName;
        }
    }
}
