using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QrssPlusDownloader
{
    public partial class FormDownloadAll : Form
    {
        readonly List<GrabUrl> GrabUrls = new();
        bool CloseNow = false;

        public FormDownloadAll()
        {
            InitializeComponent();
            FormClosed += FormDownloadAll_FormClosed;
        }

        private void FormDownloadAll_FormClosed(object sender, FormClosedEventArgs e) => CloseNow = true;

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void nudAge_ValueChanged(object sender, EventArgs e) => UpdateListOfUrls();

        private void Interactive(bool enable)
        {
            lbFiles.Enabled = enable;
            btnDownload.Enabled = enable;
            nudAge.Enabled = enable;
            Application.DoEvents();
        }

        private void FormDownloadAll_Load(object sender, EventArgs e)
        {
            lblFiles.Text = $"Loading...";
            lbFiles.Items.Clear();
            Interactive(false);

            GrabUrls.Clear();
            GrabUrls.AddRange(Downloader.DownloadGrabURLs().Select(x => new GrabUrl(x)));
            UpdateListOfUrls();

            Interactive(true);
        }

        private void UpdateListOfUrls()
        {
            lbFiles.Items.Clear();
            foreach (GrabUrl item in GrabUrls)
            {
                if (item.Age <= TimeSpan.FromMinutes((int)nudAge.Value))
                    lbFiles.Items.Add(item);
            }
            lblFiles.Text = $"Files to download ({lbFiles.Items.Count:N0}):";
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog diag = new();
            if (diag.ShowDialog() != DialogResult.OK)
                return;
            string saveFolder = diag.SelectedPath;

            pbProgress.Maximum = lbFiles.Items.Count;
            Interactive(false);

            for (int i = 0; i < lbFiles.Items.Count; i++)
            {
                if (CloseNow)
                    return;

                lbFiles.SelectedIndex = i;

                GrabUrl grabUrl = (GrabUrl)lbFiles.Items[i];
                string url = grabUrl.Url;
                string saveAs = System.IO.Path.Combine(saveFolder, grabUrl.FileName);

                using WebClient client = new();
                client.DownloadFile(url, saveAs);
                System.Diagnostics.Debug.WriteLine($"Saved: {saveAs}");

                pbProgress.Value = i;
                Application.DoEvents();
            }

            System.Diagnostics.Process.Start("explorer.exe", saveFolder);

            Interactive(true);
        }
    }
}
