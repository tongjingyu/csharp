using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace 取颜色
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int R=255, G=255, B=255;
        private int RU = 5, GU = 6, BU = 5;
        private void Form1_Load(object sender, EventArgs e)
        {
            SetColorToButton();
        }
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            SetColorToButton();
        }

        private void hScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            
            SetColorToButton();
        }

        private void hScrollBar3_ValueChanged(object sender, EventArgs e)
        {
            
            SetColorToButton();
        }
        private void SetColorToButton()
        {
            int C;
            RU = int.Parse(textBox3.Text.Trim());
            GU = int.Parse(textBox4.Text.Trim());
            BU = int.Parse(textBox5.Text.Trim());
            R = hScrollBar1.Value;
            G = hScrollBar2.Value;
            B = hScrollBar3.Value;
            button1.Text = R.ToString();
            button8.Text = G.ToString();
            button2.Text = B.ToString();
            button2.BackColor = Color.FromArgb(0, 0, B);
            button8.BackColor = Color.FromArgb(0, G, 0);
            button1.BackColor = Color.FromArgb(R, 0, 0);
            textBox1.BackColor = Color.FromArgb(R, G, B);
            textBox2.BackColor = Color.FromArgb(R, G, B);
            textBox1.ForeColor= Color.FromArgb(255-R, 255-G, 255-B);
            textBox2.ForeColor = Color.FromArgb(255 - R, 255 - G, 255 - B);
            textBox1.Text = "WinColor:" + Color.FromArgb(R, G, B).ToString();
            C = 0;
            C = (R * (int)Math.Pow(2, RU) / 256); C <<= GU;
            C += (G * (int)Math.Pow(2, GU) / 256); C <<= BU;
            C+= (B*(int)Math.Pow(2, BU)/256); 
            textBox2.Text = "TFT:0x" + C.ToString("X");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox1.BackColor = ColorPickerManager.GetColor(MousePosition.X, MousePosition.Y);
                hScrollBar1.Value = textBox1.BackColor.R;
                hScrollBar2.Value= textBox1.BackColor.G;
                hScrollBar3.Value=textBox1.BackColor.B;
                SetColorToButton();
            }
        }
    }
}
