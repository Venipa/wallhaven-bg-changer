using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallhaven_BG_Changer.API
{
    public class WallhavenSearch
    {
        [JsonProperty("data")]
        public List<WallhavenSearchItem> data { get; set; }
        [JsonProperty("meta")]
        public WallhavenSearchMeta meta { get; set; }
    }
    public class WallhavenSearchItem
    {
        [JsonProperty]
        public string id { get; set; }
        [JsonProperty]
        public string url { get; set; }
        [JsonProperty]
        public string short_url { get; set; }
        [JsonProperty]
        public int views { get; set; }
        [JsonProperty]
        public int favorites { get; set; }
        [JsonProperty]
        public string source { get; set; }
        [JsonProperty]
        public string purity { get; set; }
        [JsonProperty]
        public string category { get; set; }
        [JsonProperty]
        public int dimension_x { get; set; }
        [JsonProperty]
        public int dimension_y { get; set; }
        [JsonProperty]
        public string resolution { get; set; }
        [JsonProperty]
        public string ratio { get; set; }
        [JsonProperty]
        public int file_size { get; set; }
        [JsonProperty]
        public string file_type { get; set; }
        [JsonProperty]
        public string created_at { get; set; }
        [JsonProperty]
        public string[] colors { get; set; }
        [JsonProperty]
        public string path { get; set; }
        public WallhavenSearchItemThumbs thumbs { get; set; }
    }

    public class WallhavenSearchItemThumbs
    {
        [JsonProperty]
        public string large { get; set; }
        [JsonProperty]
        public string original { get; set; }
        [JsonProperty]
        public string small { get; set; }
    }

    public class WallhavenSearchMeta
    {
        [JsonProperty]
        public int current_page { get; set; }
        [JsonProperty]
        public int last_page { get; set; }
        [JsonProperty]
        public int per_page { get; set; }
        [JsonProperty]
        public int total { get; set; }
        [JsonProperty]
        public object query { get; set; }
        [JsonProperty]
        public object seed { get; set; }
    }

}
