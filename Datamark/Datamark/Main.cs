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
using System.Diagnostics;
using ThoughtWorks.QRCode.Codec;
namespace Datamark
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        string CodeID;
        public static void HttpDownloadFile()
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create("http://www.trtos.com/update/更新程序.exe.jpg") as HttpWebRequest;

                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();

                //创建本地文件写入流
                Stream stream = new FileStream("更新程序.exe", FileMode.Create);

                byte[] bArr = new byte[10000];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
            }
            catch { }
        }
        private void UpLoad()
        {
            try
            {
                MySql.ConnectSql();
            //    HttpDownloadFile();
                ProcessStartInfo ps = new ProcessStartInfo("上传程序.exe");
                ps.UseShellExecute = false;
                ps.CreateNoWindow = true;
                ps.Arguments = "Datamark Datamark.exe";
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(ps);
            }
            catch { }
        }
        private void Main_Load(object sender, EventArgs e)
        {
            删除ToolStripMenuItem1_Click(null, null);
            new Thread(UpLoad).Start();
        }
        public Bitmap Create_ImgCode(string codeNumber, int size)
        {
            //创建二维码生成类
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            //设置编码模式
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //设置编码测量度
            qrCodeEncoder.QRCodeScale = size;
            //设置编码版本
            qrCodeEncoder.QRCodeVersion = 0;
            //设置编码错误纠正
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //生成二维码图片
            System.Drawing.Bitmap image = qrCodeEncoder.Encode(codeNumber);
            return image;
        }
        public void SaveImg(string file, Bitmap img)
        {
                img.Save(file, System.Drawing.Imaging.ImageFormat.Png);
        }
        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tableLayoutPanel1.Enabled = false;
            linkLabel1.Text = "便签ID:";
            linkLabel1.Tag = "http://www.trtos.com/";
            pictureBox1.BackgroundImage = Properties.Resources.code;
        }

        private void 新建ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string ID=MySql.GetID();
            if (ID == null)
            {
                删除ToolStripMenuItem1_Click(null, null);
                return;
            }
            linkLabel1.Text = "便签ID:" + ID;
            CodeID = ID;
            tableLayoutPanel1.Enabled = true;
            string DataCode = "http://www.trtos.com/php/s.php?id="+ ID;
            linkLabel1.Tag = DataCode;
            Bitmap bs = Create_ImgCode(DataCode, 8);
            pictureBox1.BackgroundImage = bs;
            richTextBox2.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Tag.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            if (textBox2.Text == "") return;
            richTextBox2.AppendText(textBox1.Text + ":" + textBox2.Text + "\r\n");
            MySql.Instar(CodeID, textBox1.Text, textBox2.Text);
            textBox1.Clear();
            textBox2.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "png文件(*.png)|*.png";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackgroundImage.Save(sfd.FileName);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ProcessStartInfo ps = new ProcessStartInfo("更新程序.exe");
                ps.UseShellExecute = false;
                ps.CreateNoWindow = true;
                ps.Arguments = "Datamark";
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(ps);
            }
            catch { }
        }

        private void 退出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            uint x = 50;
            PrintLab.OpenPort(255);//打开打印机端口
            PrintLab.PTK_ClearBuffer();           //清空缓冲区
            PrintLab.PTK_SetPrintSpeed(1);        //设置打印速度
            PrintLab.PTK_SetDarkness(60);         //设置打印黑度
            PrintLab.PTK_SetLabelHeight(160, 19, 0, false); //设置标签的高度和定位间隙\黑线\穿孔的高度
            PrintLab.PTK_SetLabelWidth(520);      //设置标签的宽度
            for (int i = 1; i <= 1; i++)
            {
                //for (int z = 0; z < 4; z++)
                //{
                //    PrintLab.PTK_DrawTextTrueTypeW(400 + (int)x, 24 + 24 * z, 26, 0, "宋体", 1, 400, false, false, false, z.ToString(), CodeID.Substring(z * 4, 4));
                //}
                //for (int z = 0; z < 4; z++)
                //{
                //    PrintLab.PTK_DrawTextTrueTypeW(145 + (int)x, 24 + 24 * z, 26, 0, "宋体", 1, 400, false, false, false, z.ToString(), CodeID.Substring(z * 4, 4));
                //}
                PrintLab.PTK_DrawBar2D_QR(x, 8, 300, 300, 0, 4, 2, 0, 0, linkLabel1.Tag.ToString());
                PrintLab.PTK_DrawBar2D_QR(252 + x, 8, 300, 300, 0, 4, 2, 0, 0, linkLabel1.Tag.ToString());
                PrintLab.PTK_PrintLabel(1, 1);
            }
            PrintLab.ClosePort();//关闭打印机端口
            MessageBox.Show(linkLabel1.Tag.ToString(), "打印内容");
        }

        private void 查看帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.trtos.com/?p=532");
        }

        private void 关于DatamarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void 进入官网ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.trtos.com/");
        }
    }
}
