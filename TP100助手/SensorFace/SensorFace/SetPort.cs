using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
namespace SensorFace
{
    public partial class SetPort : Form
    {
        public SetPort()
        {
            InitializeComponent();
        }

        private void 设置_Load(object sender, EventArgs e)
        {
            string[] SPNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            for (int i = 0; i < SPNames.Length; i++)
            {
                comboBox1.Items.Add(SPNames[i]);
            }
            try
            {
                comboBox1.Text = Ini.Read("COM");
                if(comboBox1.Text.Length<3)
                comboBox1.Text =comboBox1.Items[0].ToString();
                
            }
            catch {}
        }

        private void button1_Click(object sender, EventArgs e)
        {
              Ini.Write("COM",comboBox1.Text);
              this.Close();
        }
    }
}
