using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Configs;
using NS_BMP_Tools;
namespace 毛毛虫
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public Bitmap originImg;
        private void Form1_Load(object sender, EventArgs e)
        {
            BMP_Tools BT = new BMP_Tools();
            BT.GetImage(Ini.Read("PICPATH"));
            pictureBox9.BackgroundImage = BT.SBMP;
            pictureBox9.Width = BT.SBMP.Width;
            pictureBox9.Height = BT.SBMP.Height;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = "*.*";
            Dlg.Filter = "(*.*)|*.jpg|*.bmp|*.png";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            Ini.Write("PICPATH", Dlg.FileName);
            BMP_Tools BT = new BMP_Tools();
            BT.GetImage(Ini.Read("PICPATH"));
            pictureBox9.BackgroundImage = BT.SBMP;
            pictureBox9.Width = BT.SBMP.Width;
            pictureBox9.Height = BT.SBMP.Height;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BMP_Tools BT = new BMP_Tools();
            BT.GetImage(Ini.Read("PICPATH"));
            pictureBox2.BackgroundImage=BT.CutButtonImage(int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            textBox2.Text = (int.Parse(textBox2.Text)+1).ToString();
        }
        private void pictureBox9_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(e.Location.ToString());
            BMP_Tools BT = new BMP_Tools();
            BT.SBMP = (Bitmap)pictureBox9.BackgroundImage;
            Rectangle Rec = BT.GetSelectRectangle(e.X, e.Y);
            try
            {
                textBox1.Text = Rec.X.ToString();
                textBox2.Text = Rec.Y.ToString();
                textBox3.Text = Rec.Width.ToString();
                textBox4.Text = Rec.Height.ToString();
                pictureBox2.BackgroundImage = BT.Cut(Rec);
            }
            catch { }
        }

    }
}
