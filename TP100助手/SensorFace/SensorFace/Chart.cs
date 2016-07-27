using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
namespace SensorFace
{
    public partial class Chart : Form
    {
        public Chart(string Str)
        {
            StringText = Str;
            InitializeComponent();
        }
        string StringText;
        Bitmap img;
        Graphics G;
        float[] receivebuf = new float[100];
        float[] oldreceivebuf = new float[100];
        private void TGui_Draw_Line(Bitmap image, int x1, int y1, int x2, int y2, Color color)
        {
            int dx, dy, e;
            dx = x2 - x1;
            dy = y2 - y1;

            if (dx >= 0)
            {
                if (dy >= 0) // dy>=0
                {
                    if (dx >= dy) // 1/8 octant
                    {
                        e = dy - dx / 2;
                        while (x1 <= x2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { y1 += 1; e -= dx; }
                            x1 += 1;
                            e += dy;
                        }
                    }
                    else        // 2/8 octant
                    {
                        e = dx - dy / 2;
                        while (y1 <= y2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { x1 += 1; e -= dy; }
                            y1 += 1;
                            e += dx;
                        }
                    }
                }
                else           // dy<0
                {
                    dy = -dy;   // dy=abs(dy)

                    if (dx >= dy) // 8/8 octant
                    {
                        e = dy - dx / 2;
                        while (x1 <= x2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { y1 -= 1; e -= dx; }
                            x1 += 1;
                            e += dy;
                        }
                    }
                    else        // 7/8 octant
                    {
                        e = dx - dy / 2;
                        while (y1 >= y2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { x1 += 1; e -= dy; }
                            y1 -= 1;
                            e += dx;
                        }
                    }
                }
            }
            else //dx<0
            {
                dx = -dx;        //dx=abs(dx)
                if (dy >= 0) // dy>=0
                {
                    if (dx >= dy) // 4/8 octant
                    {
                        e = dy - dx / 2;
                        while (x1 >= x2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { y1 += 1; e -= dx; }
                            x1 -= 1;
                            e += dy;
                        }
                    }
                    else        // 3/8 octant
                    {
                        e = dx - dy / 2;
                        while (y1 <= y2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { x1 -= 1; e -= dy; }
                            y1 += 1;
                            e += dx;
                        }
                    }
                }
                else           // dy<0
                {
                    dy = -dy;   // dy=abs(dy)

                    if (dx >= dy) // 5/8 octant
                    {
                        e = dy - dx / 2;
                        while (x1 >= x2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { y1 -= 1; e -= dx; }
                            x1 -= 1;
                            e += dy;
                        }
                    }
                    else        // 6/8 octant
                    {
                        e = dx - dy / 2;
                        while (y1 >= y2)
                        {
                            image.SetPixel(x1, y1, color);
                            if (e > 0) { x1 -= 1; e -= dy; }
                            y1 -= 1;
                            e += dx;
                        }
                    }
                }
            }
        }
        double Opera_WhithADCFloat(double m, float moshu1, float shuzhi1, float moshu2, float shuzhi2, float xianzhi)
        {
            if (m < moshu1)
            {
                m = moshu1 - m;
                m = (double)m * (shuzhi2 - shuzhi1) / (moshu2 - moshu1);
                m = shuzhi1 - m;
                if (m > shuzhi1) m = 0;
                return (m);
            }
            if (m > moshu2)
            {
                m = m - moshu2;
                m = (double)m * (shuzhi2 - shuzhi1) / (moshu2 - moshu1);
                m += shuzhi2;
                if (m > xianzhi) m = xianzhi;
                return (m);
            }
            if ((m >= moshu1) && (m <= moshu2))
            {
                m = m - moshu1;
                m = (double)m * (shuzhi2 - shuzhi1) / (moshu2 - moshu1);
                m = m + shuzhi1;
                if (m > xianzhi) m = xianzhi;
                return (m);
            }
            return 0;
        }
        private int WriteChart(Bitmap image, Graphics G, float[] Bytes, Color C, bool Clear)
        {
        try {
                int x, y = 0, Max=-100,Min=1000;
                float Grid = (float)image.Width / (float)Bytes.Length;
                for (int n = 0; n < (Bytes.Length - 1); n++)
                {
                    if (Bytes[n] > Max) Max = (int)Bytes[n];
                    if (Bytes[n] < Min) Min = (int)Bytes[n];
                    
                }
                for (int i = 0; i < (Bytes.Length-1); i++)
                {
                    int a = (int)Opera_WhithADCFloat(Bytes[i], Min, 0, Max, image.Height-1,image.Height);
                    int b = (int)Opera_WhithADCFloat(Bytes[i+1], Min, 0, Max, image.Height-1, image.Height);
                    if (a > (image.Height-10)) a = image.Height - 1;
                    if (a < 0) a = 1;
                    if (b > (image.Height-10)) b = image.Height - 1;
                    if (b < 0) b = 1;
                    try
                    {
                        x = (int)(Grid * (float)i);
                        TGui_Draw_Line(img, x, image.Height - 1 - y - a, x + (int)Grid, image.Height - 1 - y - b, C);
                    }
                    catch (Exception E) {}
                
                }
                return Max;
            }
            catch(Exception E){MessageBox.Show(E.Message); return -1; }
        }
        private float[] GetBytesFromString(string Text,int Index)
        {
            string[] arrString = Text.Split('\n');
            int i=0;
            float[] Data = new float[1000];
            for(int n=0;n<arrString.Length;n++)
            {
                string[] All = arrString[n].Split(',');
                try
                {
                    Data[i] =float.Parse(All[Index]);

                    i++;
                }
                catch (Exception E)
                {
                }
            }
            return Data.Skip(0).Take(i).ToArray() ;
        }
        private void WriteString(Graphics G,Color C,uint x,uint y,string msg)
        {
            String str = msg;
            Font font = new Font("宋体", 9);
            SolidBrush sbrush = new SolidBrush(C);
            G.DrawString(str, font, sbrush, new PointF(x, y));
        }
        private void 过程图_Load(object sender, EventArgs e)
        {
            try
            {
                img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                G = Graphics.FromImage(img);
               // WriteChart(img, G, oldreceivebuf, Color.FromArgb(60, 60, 60), false);
                receivebuf = GetBytesFromString(StringText, 1);
                WriteString(G, Color.Green, 10, 14, "温度 " + WriteChart(img, G, receivebuf, Color.Green, false));
                receivebuf = GetBytesFromString(StringText, 2);
                WriteString(G, Color.Red, 10, 30, "湿度 " + WriteChart(img, G, receivebuf, Color.Red, false));
                pictureBox1.Image = img;
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
        }

        private void 过程图_SizeChanged(object sender, EventArgs e)
        {
            过程图_Load(null, null);
        }

        private void 导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "bmp files(*.bmp)|*.bmp|All files(*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                bmp.Save(sfd.FileName);   
            }
            
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
                return;
            }
        }

        private void 打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog MyPrintDg = new PrintDialog();
            MyPrintDg.Document = printDocument1;
            if (MyPrintDg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    printDocument1.Print();
                }
                catch
                {   //停止打印
                    printDocument1.PrintController.OnEndPrint(printDocument1, new System.Drawing.Printing.PrintEventArgs());
                }
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image, pictureBox1.Width,pictureBox1.Height);
        }

    }
}
