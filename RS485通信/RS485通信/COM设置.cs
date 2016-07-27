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
namespace Geek
{
    public partial class COM设置 : Form
    {
        public bool SetTrue = false;
        public bool FritsLoad;
        public COM设置(bool FritsLoad)
        {
            this.FritsLoad = FritsLoad;
            InitializeComponent();
        }
        private void COM设置_Load(object sender, EventArgs e)
        {
            Value.ComName = Ini.Read("COM_Name");
            comboBox1.Text = Value.ComName;
            if (int.TryParse(Ini.Read("COM_BaudRate"), out Value.COM_BaudRate)) comboBox2.Text = Ini.Read("COM_BaudRate");
            if (int.TryParse(Ini.Read("Device_Addr"), out Value.DeviceAddr)) comboBox3.Text = Ini.Read("Device_Addr");
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
            if (int.TryParse(comboBox2.Text, out temp)==false) MessageBox.Show("无效数据"); 
            if (int.TryParse(comboBox3.Text, out temp) == false)  MessageBox.Show("无效数据"); 
            Ini.Write("COM_Name", comboBox1.Text);
            Ini.Write("COM_BaudRate", comboBox2.Text);
            Ini.Write("Device_Addr", comboBox3.Text);
            this.Close();
        }
    }
}
