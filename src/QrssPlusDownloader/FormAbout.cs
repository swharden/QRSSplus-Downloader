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
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            Version ver = typeof(FormMain).Assembly.GetName().Version;
            lblVersion.Text = ver.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/swharden/QRSSplus-Downloader");
        }
    }
}
