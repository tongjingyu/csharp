using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SensorFace
{
    public partial class GetInput : Form
    {
        string Msg;
       string NameString;
        public GetInput(string Msg,string NameString)
        {
            this.Msg = Msg;
            this.NameString = NameString;
            InitializeComponent();
        }
     
        private void IpPut_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Msg;
            this.Text = NameString;
            textBox1.KeyDown += textBox1_KeyDown;
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                this.Text = textBox1.Text;
                this.DialogResult = DialogResult.OK;
            }if(e.KeyCode==Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Text = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
