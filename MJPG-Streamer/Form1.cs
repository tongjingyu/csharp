using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace MJPG_Streamer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap bmp;
        bool Run = true;

        private void Form1_Load(object sender, EventArgs e)
        {
            new Thread(showVideo).Start();
        }
        private void showVideo()
        {
            while (Run)
            {
                byte[] buffer = new byte[100000];
                int read, total = 0;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://192.168.1.34:8080/?action=snapshot");
                //req.Credentials = new NetworkCredential("root", "admin");
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                while ((read = stream.Read(buffer, total, 100)) != 0)
                {
                    total += read;
                }
                bmp = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, 0, total));
                Thread.Sleep(100);
                timer1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            pictureBox1.Image = bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = bmp;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Run = false;
        }
    }
}
