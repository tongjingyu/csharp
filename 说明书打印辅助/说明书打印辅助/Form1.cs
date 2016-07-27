using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 说明书打印辅助
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        uint PageLength;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                PageLength = uint.Parse(textBox1.Text);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }

        }
    }
}
