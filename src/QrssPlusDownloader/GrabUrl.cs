using System;

namespace QrssPlusDownloader;

public class GrabUrl
{
    public string Url { get; private set; }
    public string FileName => System.IO.Path.GetFileName(Url);
    public GrabUrl(string url) { Url = url; }
    public override string ToString() => FileName;
    public string GrabberID => FileName.Split(' ')[0];
    public TimeSpan Age => DateTime.UtcNow - DateTime;
    public DateTime DateTime
    {
        get
        {
            string[] parts = FileName.Split(' ')[1].Split('.');
            if (parts.Length != 6)
                throw new InvalidOperationException($"Invalid timestamp in: {FileName}");

            return new DateTime(
                year: int.Parse(parts[0]),
                month: int.Parse(parts[1]),
                day: int.Parse(parts[2]),
                hour: int.Parse(parts[3]),
                minute: int.Parse(parts[4]),
                second: int.Parse(parts[5]));
        }
    }
}
