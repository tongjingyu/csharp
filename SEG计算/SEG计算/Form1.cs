using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEG计算
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte Data = 0x00;
            if (checkBox1.Checked) Data |= ((byte)(1 << int.Parse(textBox1.Text)));
            if (checkBox2.Checked) Data |= ((byte)(1 << int.Parse(textBox2.Text)));
            if (checkBox3.Checked) Data |= ((byte)(1 << int.Parse(textBox3.Text)));
            if (checkBox4.Checked) Data |= ((byte)(1 << int.Parse(textBox4.Text)));
            if (checkBox5.Checked) Data |= ((byte)(1 << int.Parse(textBox5.Text)));
            if (checkBox6.Checked) Data |= ((byte)(1 << int.Parse(textBox6.Text)));
            if (checkBox7.Checked) Data |= ((byte)(1 << int.Parse(textBox7.Text)));
            if (checkBox8.Checked) Data |= ((byte)(1 << int.Parse(textBox9.Text)));
            byte temp = Data;
            String ts="   ";
            for (int i = 0; i < 8; i++)
            {
                if ((temp & (byte)0x80) > 0) ts += "X"; else ts += "_";
                temp <<= 1;
            }
            textBox8.Text = Data.ToString("x2")+ts;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Ini.Read("a");
            textBox3.Text = Ini.Read("b");
            textBox6.Text = Ini.Read("c");
            textBox7.Text = Ini.Read("d");
            textBox5.Text = Ini.Read("e");
            textBox2.Text = Ini.Read("f");
            textBox4.Text = Ini.Read("g");
            textBox9.Text = Ini.Read("h");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("a", textBox1.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("b", textBox3.Text);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("c", textBox6.Text);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("d", textBox7.Text);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("e", textBox5.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("f", textBox2.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("g", textBox4.Text);
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("h", textBox9.Text);
        }
    }
}
