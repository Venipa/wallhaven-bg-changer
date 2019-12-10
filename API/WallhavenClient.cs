using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestRequest = RestSharp.Serializers.Newtonsoft.Json.RestRequest;

namespace Wallhaven_BG_Changer.API
{
    public class WallhavenClient
    {
        private string apiKey { get; set; }
        private RestClient client { get; set; }
        public WallhavenClient(string apiKey = null)
        {
            this.apiKey = apiKey;
            this.client = new RestClient("https://wallhaven.cc/api/v1/");
        }
        public async Task<WallhavenSearch> search(string query, string sorting = "toplist", string toprange = "1M", bool random = false)
        {
            var rr = new RestRequest("search", Method.GET);
            if (query != null)
            {
                rr.AddQueryParameter("q", query);
            }
            if (random)
            {
                rr.AddQueryParameter("seed", Guid.NewGuid().ToString("n"));
            }
            rr.AddQueryParameter("sorting", sorting);
            rr.AddQueryParameter("topRange", toprange);
            return this.client.Execute(rr)?.Content?.DeserializeObject<WallhavenSearch>();
        }
        public async Task<byte[]> downloadWallpaper(WallhavenSearchItem item)
        {
            var client = new RestClient();
            return client.DownloadData(new RestRequest(item.path, Method.GET));
        }
    }
    public static class Utils
    {
        public static T DeserializeObject<T>(this string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }
        public static string SerializeObject(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
