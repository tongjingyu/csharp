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
    public partial class Form2 : Form
    {
        private string Str;
        public Form2(string Str)
        {
            this.Str = Str;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = Str;
        }


    }
}
