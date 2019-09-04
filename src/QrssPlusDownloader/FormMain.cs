using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QrssPlusDownloader
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (lblTime.Text == "time")
            {
                // program just opened
                UpdateGrabberList();
            }

            string timeStringNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (lblTime.Text != timeStringNow)
            {
                lblTime.Text = timeStringNow;
                if (automaticToolStripMenuItem.Checked)
                {
                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    int epochSec = (int)t.TotalSeconds;
                    int secondsInTenMinutes = 60 * 10;
                    int secSinceLastTenMinutes = epochSec % secondsInTenMinutes;
                    int secTillNextTenMinutes = secondsInTenMinutes - secSinceLastTenMinutes;
                    lblStatus.Text = $"next download will start in {secTillNextTenMinutes} seconds...";
                }

                if (timeStringNow.EndsWith("0:00"))
                {
                    if (automaticToolStripMenuItem.Checked)
                    {
                        DownloadNow();
                    }
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LaunchQRSSPlusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://swharden.com/qrss/plus");
        }

        private void UpdateGrabberListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateGrabberList();
        }

        private void NowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadNow();
        }

        private void AutomaticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            automaticToolStripMenuItem.Checked = !automaticToolStripMenuItem.Checked;
        }

        private void UpdateGrabberList()
        {
            lblStatus.Text = "Downloading index of latest grabs...";
            Application.DoEvents();
            string[] urls = Downloader.DownloadGrabURLs();
            string[] calls = Downloader.GetCallSigns(urls);
            lbGrabbers.Items.Clear();
            lbGrabbers.Items.AddRange(calls);
            lblStatus.Text = $"Found {calls.Length} grabbers";
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.InitialDirectory = System.IO.Path.GetFullPath("./");
            diag.FileName = "graberIDs.txt";
            diag.Filter = "TXT Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                string txt = "";
                foreach (Object item in lbGrabbers.CheckedItems)
                    txt += item.ToString() + "\n";
                System.IO.File.WriteAllText(diag.FileName, txt);
                lblStatus.Text = $"Saved {diag.FileName}";
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = System.IO.Path.GetFullPath("./");
            diag.Filter = "TXT Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                string[] lines = System.IO.File.ReadAllLines(diag.FileName);
                lblStatus.Text = $"Loaded {diag.FileName}";

                for (int i = 0; i < lbGrabbers.Items.Count; i++)
                {
                    if (lines.Contains(lbGrabbers.Items[i].ToString()))
                        lbGrabbers.SetItemChecked(i, true);
                    else
                        lbGrabbers.SetItemChecked(i, false);
                }
            }
        }

        private void LockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbGrabbers.Enabled = false;
        }

        private void UnlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbGrabbers.Enabled = true;
        }

        private void OpenDownloadsFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pathHere = System.IO.Path.GetFullPath("./");
            string pathDownloads = System.IO.Path.GetFullPath("./downloads");
            if (System.IO.Directory.Exists(pathDownloads))
                System.Diagnostics.Process.Start("explorer.exe", pathDownloads);
            else
                System.Diagnostics.Process.Start("explorer.exe", pathHere);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout frm = new FormAbout();
            frm.ShowDialog();
        }

        private void DownloadNow()
        {
            bool USE_EXCEPTIONS = true;

            if (USE_EXCEPTIONS)
            {
                try
                {
                    ListFilesNeeded();
                    DownloadFilesListed();
                }
                catch
                {
                    lblStatus.Text = "ERROR: an exception was thrown in DownloadNow()";
                }
            }
            else
            {
                ListFilesNeeded();
                DownloadFilesListed();
            }
        }

        private void ListFilesNeeded()
        {

            lbFiles.Items.Clear();
            lblStatus.Text = "Getting latest list of grabs...";
            Application.DoEvents();

            List<string> callsToDownload = new List<string>();
            foreach (Object item in lbGrabbers.CheckedItems)
                callsToDownload.Add(item.ToString());

            foreach (string fileName in Downloader.DownloadGrabURLs())
                if (callsToDownload.Contains(fileName.Split('.')[0]))
                    if (!Downloader.AlreadyDownloaded(fileName))
                        lbFiles.Items.Add(fileName);

            if (callsToDownload.Count == 0)
                lblStatus.Text = "ERROR: no grabbers are checked.";
            else
                lblStatus.Text = $"Found {lbFiles.Items.Count} images that need downloading...";
        }

        private void DownloadFilesListed()
        {
            int filesToDownload = lbFiles.Items.Count;

            progress.Maximum = filesToDownload;
            for (int i = 0; i < filesToDownload; i++)
            {
                string filename = lbFiles.Items[0].ToString();
                lblStatus.Text = $"downloading {i + 1} of {filesToDownload}";
                Downloader.Download(filename);
                lbFiles.Items.RemoveAt(0);
                progress.Value = i;
                Application.DoEvents();
            }

            progress.Value = 0;
            lblStatus.Text = $"completed download of {filesToDownload} images";
        }
    }
}
