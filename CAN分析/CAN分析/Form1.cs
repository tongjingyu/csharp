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
using System.Threading;
using System.IO;

namespace CAN分析
{
    public partial class Form1 : Form
    {

        int RxTimeOutCount;
        private Usart Usart1;
        private byte DevAddr;

        StringBuilder SB = new StringBuilder();
        int RxCount = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
            button2_Click(null, null);
            Usart1 = new Usart(serialPort1);
            DevAddr = byte.Parse(comboBox3.Text);
            comboBox3.Text = Ini.Read("DEVID");
            Tools.SetDoubleBuf(listView1, true);
            listView1.Columns.Clear();
            listView1.Columns.Add("Time", listView1.Size.Width / 5, HorizontalAlignment.Left);
            listView1.Columns.Add("StdID", listView1.Size.Width / 10, HorizontalAlignment.Left);
            listView1.Columns.Add("ExtID", listView1.Size.Width / 10, HorizontalAlignment.Left);
            listView1.Columns.Add("IDE", listView1.Size.Width / 10, HorizontalAlignment.Left);
            listView1.Columns.Add("RTR", listView1.Size.Width / 2, HorizontalAlignment.Left);
            listView1.Columns.Add("DLC", listView1.Size.Width / 2, HorizontalAlignment.Left);
            listView1.Columns.Add("DATA", listView1.Size.Width / 2, HorizontalAlignment.Left);
            listView1.View = View.Details;
            listView1.BeginUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] SPNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            for (int i = 0; i < SPNames.Length; i++)
            {
                comboBox1.Items.Add(SPNames[i]);
            }
            if (SPNames.Length > 0)
            {
                comboBox1.Text = SPNames[0];
                button2.Enabled = true;
            }
            else button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count <= 0) return;
            if (button2.Text == "打开串口")
                try
                {

                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.Open();
                    button2.Text = "关闭串口";
                    button2.ForeColor = Color.Red;
                }
                catch { MessageBox.Show("端口不存在或被占用", "警告"); }
            else
                try
                {
                    button2.Text = "打开串口";
                    serialPort1.Close();
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("DEVID", comboBox3.Text);
            if (!byte.TryParse(comboBox3.Text, out DevAddr)) DevAddr = 0;
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            Ini.Write("BOOTSize", comboBox3.Text);
        }

        private void UpdateList(CanRxMsg Msg)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.ImageIndex = 0;
            lvi.Text = DateTime.Now.ToString("HH:MM:ss");
            lvi.SubItems.Add("0x"+Msg.StdId.ToString("x8"));
            lvi.SubItems.Add("0x" + Msg.ExtId.ToString("x8"));
            lvi.SubItems.Add("0x" + Msg.IDE.ToString("x2"));
            lvi.SubItems.Add("0x" + Msg.RTR.ToString("x2"));
            lvi.SubItems.Add("0x" + Msg.DLC.ToString("x2"));
            lvi.SubItems.Add(Tools.HexToString(Msg.Data,Msg.DLC));
            listView1.Items.Insert(0, lvi);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.EndUpdate();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (RxTimeOutCount-- == 0)
            {
                if (SB.Length > 5)this.textBox1.Text=(SB.ToString());
                byte[] RxBuffer = Tools.StringToHex(SB.ToString());
                if (ZigBeeBus.ZigBee_CheckCrc(RxBuffer))
                {
                    CanRxMsg Msg = CAN.CAN_Export(RxBuffer, 4);
                    bool Ok = true;
                    for (int i = 0; i < comboBox4.Items.Count; i++)
                    {
                        if (comboBox4.Items[i].ToString() =="0x"+ Msg.StdId.ToString("x8")) Ok = false;
                    }
                    if(Ok) comboBox4.Items.Add("0x" + Msg.StdId.ToString("x8"));
                    if(comboBox4.Text=="")UpdateList(Msg);
                    if (comboBox4.Text == "0x" + Msg.StdId.ToString("x8")) {
                        UpdateList(Msg);
                        label3.Text = (Msg.Data[0] * 0xff + Msg.Data[1]).ToString();
                        label4.Text = (Msg.Data[3] * 0xff + Msg.Data[2]).ToString();
                        label5.Text = (Msg.Data[5] * 0xff + Msg.Data[4]).ToString();
                        label6.Text = Tools.ByteToFloat(Msg.Data, 0, 0).ToString();
                    }
                    }
                SB.Clear();
                try { serialPort1.DiscardInBuffer(); } catch { }
               
            }
        }


        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] Buffer_Get = null;
            try
            {
                int N = this.serialPort1.BytesToRead;
                Buffer_Get = new byte[N];
                this.serialPort1.Read(Buffer_Get, 0, N);
                RxCount += N;
                RxTimeOutCount = 2;
                foreach (byte Buf in Buffer_Get)
                {
                    SB.Append(Buf.ToString("X2") + " ");
                }
            }
            catch { }
            finally
            {
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = null;
            listView1.Items.Clear();  
            listView1.EndUpdate();
        }

  
    }
}
