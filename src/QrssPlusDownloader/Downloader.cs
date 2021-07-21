using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QrssPlusDownloader
{
    public static class Downloader
    {
        private const string URL_GRABBERS_JSON = "https://qrssplus.z20.web.core.windows.net/grabbers.json";

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
                    foreach(var url in grabber.Value.GetProperty("urls").EnumerateArray())
                    {
                        urls.Add(url.GetString());
                    }
                }
            };

            return urls.ToArray();
        }

        /// <summary>
        /// Given an array of historical grab URLs, return an array of callsigns observed in the array
        /// </summary>
        public static string[] GetCallSigns(string[] urls)
        {
            HashSet<string> calls = new HashSet<string>();
            foreach (string url in urls)
            {
                string call = System.IO.Path.GetFileName(url).Split(' ')[0];
                if (!calls.Contains(call))
                    calls.Add(call);
            }
            return calls.ToArray();
        }

        public static string PathForGrabber(string filename)
        {
            string call = filename.Split('.')[0];
            string savePath = System.IO.Path.GetFullPath("./");
            savePath = System.IO.Path.Combine(savePath, "downloads");
            savePath = System.IO.Path.Combine(savePath, call);
            savePath = System.IO.Path.Combine(savePath, filename);
            return savePath;
        }

        public static bool AlreadyDownloaded(string filename)
        {
            string savePath = PathForGrabber(filename);
            string saveFolder = System.IO.Path.GetDirectoryName(savePath);

            if (!System.IO.Directory.Exists(saveFolder))
                return false;

            if (System.IO.File.Exists(savePath))
                return true;

            return false;
        }

        public static void Download(string filename, string urlBase = "http://swharden.com/qrss/plus/data/")
        {
            if (Downloader.AlreadyDownloaded(filename))
                return;

            string savePath = PathForGrabber(filename);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
            string url = urlBase + filename;
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
