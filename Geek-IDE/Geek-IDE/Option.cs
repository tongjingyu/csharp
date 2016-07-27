using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
namespace Geek_IDE
{
    public partial class Option : Form
    {
        public bool SetTrue = false;
        public bool FritsLoad;
        public Option(bool FritsLoad)
        {
            this.FritsLoad = FritsLoad;
            InitializeComponent();
        }
        private void COM设置_Load(object sender, EventArgs e)
        {
            Value.ComName = Ini.Read("COM","Name");
            comboBox1.Text = Value.ComName;
            if (int.TryParse(Ini.Read("COM","BaudRate"), out Value.COM_BaudRate)) comboBox2.Text = Ini.Read("COM","BaudRate");
            if (int.TryParse(Ini.Read("COM","Addr"), out Value.DeviceAddr)) comboBox3.Text = Ini.Read("COM","Addr");
        }
        private void Serach()
        {
            string[] SPNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            for (int i = 0; i < SPNames.Length; i++)
            {
                comboBox1.Items.Add(SPNames[i]);
            }
            if (SPNames.Length == 1)
            {
                comboBox1.Text = SPNames[0];
            }
            else if (SPNames.Length > 1)
            {
                comboBox1.Text = SPNames[0];
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Serach();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int temp;
            if (comboBox1.Text.IndexOf("COM") < 0) MessageBox.Show("无效端口");
            if (int.TryParse(comboBox2.Text, out temp) == false) MessageBox.Show("无效数据");
            if (int.TryParse(comboBox3.Text, out temp) == false) MessageBox.Show("无效数据");
            Ini.Write("COM","Name", comboBox1.Text);
            Ini.Write("COM","BaudRate", comboBox2.Text);
            Ini.Write("COM","Addr", comboBox3.Text);
            this.Close();
        }
    }
}
