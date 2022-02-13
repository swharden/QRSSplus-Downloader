using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QrssPlusDownloader
{
    public static class Downloader
    {
        private const string URL_GRABBERS_JSON = "https://qrssplus.z20.web.core.windows.net/grabbers.json";

        public static GrabUrl[] GetGrabUrls()
        {
            List<GrabUrl> grabUrls = new();

            using JsonDocument document = RequestJson(URL_GRABBERS_JSON);

            foreach (JsonProperty grabber in document.RootElement.GetProperty("grabbers").EnumerateObject())
            {
                foreach (JsonElement url in grabber.Value.GetProperty("urls").EnumerateArray())
                {
                    grabUrls.Add(new(url.ToString()));
                }
            }

            return grabUrls.ToArray();
        }

        private static JsonDocument RequestJson(string url) => RequestJsonAsync(url).Result;

        private static async Task<JsonDocument> RequestJsonAsync(string url)
        {
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            using HttpContent content = response.Content;
            string json = await content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }

        /// <summary>
        /// Return an array of image URLs for all historical grabs for all grabbers
        /// </summary>
        public static string[] DownloadGrabURLs()
        {
            List<string> urls = new List<string>();
            using (WebClient client = new WebClient())
            using (JsonDocument document = JsonDocument.Parse(client.DownloadString(URL_GRABBERS_JSON)))
            {
                foreach (JsonProperty grabber in document.RootElement.GetProperty("grabbers").EnumerateObject())
                {
                    foreach (var url in grabber.Value.GetProperty("urls").EnumerateArray())
                    {
                        urls.Add(url.GetString());
                    }
                }
            };

            return urls.ToArray();
        }

        public static string[] CallsignFromUrls(string[] urls) => urls.Select(x => CallsignFromUrl(x)).Distinct().ToArray();

        public static string CallsignFromUrl(string url) => System.IO.Path.GetFileName(url).Split(' ')[0];

        public static string PathForGrabber(string url)
        {
            string filename = System.IO.Path.GetFileName(url);
            string call = CallsignFromUrl(url); ;
            string savePath = System.IO.Path.GetFullPath("./");
            savePath = System.IO.Path.Combine(savePath, "downloads");
            savePath = System.IO.Path.Combine(savePath, call);
            savePath = System.IO.Path.Combine(savePath, filename);
            return savePath;
        }

        public static bool AlreadyDownloaded(string url)
        {
            string filename = System.IO.Path.GetFileName(url);
            string savePath = PathForGrabber(filename);
            string saveFolder = System.IO.Path.GetDirectoryName(savePath);

            if (!System.IO.Directory.Exists(saveFolder))
                return false;

            if (System.IO.File.Exists(savePath))
                return true;

            return false;
        }

        public static void Download(string url)
        {
            if (AlreadyDownloaded(url))
                return;

            string savePath = PathForGrabber(url);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, savePath);
                }
                catch
                {
                    Console.WriteLine("exception thrown in Download()");
                }
            }
        }
    }
}
