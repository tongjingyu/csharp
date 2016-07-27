using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geek_IDE
{
    public partial class AT_HELP : Form
    {
        public AT_HELP()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox1.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox2.Text);
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox3.Text);
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox4.Text);
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox5.Text);
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox6.Text);
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox7.Text);
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox8.Text);
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, textBox9.Text);
            this.Close();
        }
    }
}
