using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Geek_IDE
{
    public partial class AT指令 : Form
    {
        public AT指令()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] RxBuf = new byte[100];
            Value.Port1.WriteLine("AT\r");
            Thread.Sleep(100);
            int Length = Value.Port1.Read(RxBuf, 0, Value.Port1.BytesToRead);
            richTextBox1.AppendText(Tools.HexToString(RxBuf, Length));
            richTextBox2.AppendText(Encoding.GetEncoding("GB2312").GetString(RxBuf));
        }


        private void button4_Click(object sender, EventArgs e)
        {
            AT_HELP form = new AT_HELP();
            form.ShowDialog();
            button2_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = Clipboard.GetData(DataFormats.Text).ToString();
                byte[] RxBuf = new byte[100];
                Value.Port1.WriteLine(textBox1.Text + "\r");
                Thread.Sleep(100);
                int Length = Value.Port1.Read(RxBuf, 0, Value.Port1.BytesToRead);
                richTextBox1.AppendText(Tools.HexToString(RxBuf, Length));
                richTextBox2.AppendText(Encoding.GetEncoding("GB2312").GetString(RxBuf));
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Value.Port1.Open();
            }
            catch { }
            byte[] RxBuf = new byte[100];
            Value.Port1.WriteLine(textBox1.Text + "\r");
            Thread.Sleep(100);
            int Length = Value.Port1.Read(RxBuf, 0, Value.Port1.BytesToRead);
            richTextBox1.AppendText(Tools.HexToString(RxBuf, Length));
            richTextBox2.AppendText(Encoding.GetEncoding("GB2312").GetString(RxBuf));

            try
            {
                Value.Port1.Close();
            }
            catch { }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "AT+CWSAP=\"" + textBox2.Text + "\",\"" + textBox3.Text + "\",1,\"" + textBox4.Text + "\"";
            button1_Click(null, null);
        }
    }
}
