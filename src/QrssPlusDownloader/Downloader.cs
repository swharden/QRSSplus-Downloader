using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QrssPlusDownloader
{
    public static class Downloader
    {
        public static string[] DownloadGrabURLs(string urlGrabberIndex = "http://swharden.com/qrss/plus/data/")
        {
            WebClient client = new WebClient();
            List<string> urls = new List<string>();
            string[] imageExtensions = { "jpg", "png", "gif" };
            string index = client.DownloadString(urlGrabberIndex);
            foreach (string possibleUrl in index.Split(new string[] { "href=" }, StringSplitOptions.None))
            {
                if (!possibleUrl.StartsWith("\""))
                    continue;
                string url = possibleUrl.Split('\"')[1];
                string ext = System.IO.Path.GetExtension(url).Trim('.').ToLower();
                if (ext == "")
                    continue;
                if (imageExtensions.Contains(ext))
                    urls.Add(url);
            }
            client.Dispose();
            return urls.ToArray();
        }

        public static string[] GetCallSigns(string[] urls)
        {
            List<string> calls = new List<string>();
            foreach (string url in urls)
            {
                string[] parts = url.Split('.');
                string call = parts[0];
                string timestamp = parts[1];
                string hash = parts[2];
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
            return System.IO.File.Exists(PathForGrabber(filename));
        }

        public static void Download(string filename, string urlBase = "http://swharden.com/qrss/plus/data/")
        {
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
