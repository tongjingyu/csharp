using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 字符编码
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = Ini.Read("WordRecord");
            textBox5.Text = Ini.Read("Record1");
            textBox6.Text = Ini.Read("Record2");
            textBox8.Text = Ini.Read("Record3");
        }





        private void button7_Click(object sender, EventArgs e)
        {

            byte[] Buf = new byte[2];

            String Str = "";
            for (int j = 0x81; j < 0xff; j++)
            {
                Buf[0] = (byte)j;
                for (int i = 0x80; i < 0xff; i++)
                {
                    Buf[1] = (byte)i;
                    Str += Encoding.GetEncoding("GB2312").GetString(Buf);
                }
            }
            Form2 form = new Form2(Str);
            form.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[1];
            String Str = "";
            for (int j = 0x01; j < 0x80; j++)
            {

                Buf[0] = (byte)j;
                Str += Encoding.GetEncoding("ASCII").GetString(Buf);
            }
            Form2 form = new Form2(Str);
            form.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[2];

            String Str = "";
            for (int j = 0x81; j < 0xff; j++)
            {
                Buf[0] = (byte)j;
                for (int i = 0x80; i < 0xff; i++)
                {
                    Buf[1] = (byte)i;
                    Str += Encoding.GetEncoding("BIG5").GetString(Buf);
                }
            }
            Form2 form = new Form2(Str);
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] Buf = Encoding.GetEncoding(label2.Text).GetBytes(richTextBox1.Text);
                textBox2.Text = Tools.HexToString(Buf, Buf.Length);
                Buf = Encoding.GetEncoding(label3.Text).GetBytes(richTextBox1.Text);
                textBox3.Text = Tools.HexToString(Buf, Buf.Length);
                Buf = Encoding.GetEncoding(textBox5.Text).GetBytes(richTextBox1.Text);
                textBox4.Text = Tools.HexToString(Buf, Buf.Length);
                Ini.Write("Record1", textBox5.Text);
                Buf = Encoding.GetEncoding(textBox6.Text).GetBytes(richTextBox1.Text);
                textBox7.Text = Tools.HexToString(Buf, Buf.Length);
                Ini.Write("Record2", textBox6.Text);
                Buf = Encoding.GetEncoding(textBox8.Text).GetBytes(richTextBox1.Text);
                textBox9.Text = Tools.HexToString(Buf, Buf.Length);
                Ini.Write("Record3", textBox8.Text);
            }
            catch (Exception E) { MessageBox.Show(E.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] Buf = Tools.StringToHex(textBox2.Text);
            richTextBox1.Text = Encoding.GetEncoding(label2.Text).GetString(Buf);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Buf = Tools.StringToHex(textBox3.Text);
            richTextBox1.Text = Encoding.GetEncoding(label3.Text).GetString(Buf);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] Buf = Tools.StringToHex(textBox4.Text);
            richTextBox1.Text = Encoding.GetEncoding(textBox5.Text).GetString(Buf);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] Buf = Tools.StringToHex(textBox7.Text);
            richTextBox1.Text = Encoding.GetEncoding(textBox6.Text).GetString(Buf);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] Buf = Tools.StringToHex(textBox9.Text);
            richTextBox1.Text = Encoding.GetEncoding(textBox8.Text).GetString(Buf);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("WordRecord", richTextBox1.Text);
        }

    }
}
