using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace CSharp
{
    public partial class Main : Form
    {
       
        private const int ValueSize=16;
        public Main()
        {
            InitializeComponent();
        }
        MB ModBus = new MB();
        private void DataGridInit()
        {
            int i; 
            dataGridView1.Rows.Add(ValueSize);
            for (i = 0; i < ValueSize; i++) dataGridView1[0, i].Value = i;
            i=0;
            dataGridView1[1, i++].Value = "风机启控值";
            dataGridView1[1, i++].Value = "风机控制回差值";
            dataGridView1[1, i++].Value ="超温报警值";
            dataGridView1[1, i++].Value ="超温跳闸值";
            dataGridView1[1, i++].Value ="湿度设定值";
            dataGridView1[1, i++].Value ="温度修正值";
            dataGridView1[1, i++].Value ="SU窗口选项设置";
            dataGridView1[1, i++].Value ="历史最高油温";
            dataGridView1[1, i++].Value ="485通信地址";
            dataGridView1[1, i++].Value = "波特率";
            dataGridView1[1, i++].Value = "校准A点温度";
            dataGridView1[1, i++].Value = "校准A点模数";
            dataGridView1[1, i++].Value = "校准B点温度";
            dataGridView1[1, i++].Value = "校准B点模数";
            dataGridView1[1, i++].Value = "4毫安对应的pwm值";
            dataGridView1[1, i++].Value = "20毫安对应的pwm值";

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridInit();
            button1_Click(null, null);
            button2_Click(null, null);
            Tools.SetDoubleBuf(dataGridView1, true);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[100];
            ModBus.TargetAddr = ModBusClass.BroadAddr;
            ModBus.HostAddr = ModBusClass.HostAddr;
            ModBus.MsgFlag = (byte)MsgFlag.TimeLock;
            ModBus.MsgNum = 1;
            ModBus.DataLength = 8;
            ModBus.DataFlag = 0x01;
            ModBus.MsgLength = ModBus.DataLength + 3;
            ModBus.Data = Tools.DateTimeToBytes(DateTime.Now);
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            SetBoxFromStruct(ModBus);
            button3_Click(null, null);
            button4_Click(null, null);
        }
        private void GetBoxToStruct(ref MB TempMB)
        {
            TempMB.Start = ModBusClass.StartFlag;
            TempMB.TargetAddr = Tools.StringToHex(textBox1.Text)[0];
            TempMB.HostAddr = Tools.StringToHex(textBox2.Text)[0];
            TempMB.MsgFlag = Tools.StringToHex(textBox3.Text)[0];
            TempMB.MsgNum = Tools.StringToHex(textBox4.Text)[0];
            TempMB.MsgLength = Tools.StringToHex(textBox5.Text)[0];
            TempMB.MsgLength <<= 8;
            TempMB.MsgLength |= Tools.StringToHex(textBox5.Text)[1];
            if (TempMB.MsgLength >= 3)
            {
                TempMB.DataFlag = Tools.StringToHex(textBox6.Text)[0];
                TempMB.DataLength = Tools.StringToHex(textBox7.Text)[0];
                TempMB.DataLength <<= 8;
                TempMB.DataLength |= Tools.StringToHex(textBox7.Text)[1];
                TempMB.Data = Tools.StringToHex(textBox8.Text);
            }
        }
        private void SetBoxFromStruct(MB TempMB)
        {
            textBox1.Text = TempMB.TargetAddr.ToString("X2");
            textBox2.Text = TempMB.HostAddr.ToString("X2");
            textBox3.Text = TempMB.MsgFlag.ToString("X2");
            textBox4.Text = TempMB.MsgNum.ToString("X2");
            textBox5.Text = TempMB.MsgLength.ToString("X4");
            if (ModBus.MsgLength >= 3)
            {
                textBox6.Text = TempMB.DataFlag.ToString("X2");
                textBox7.Text = TempMB.DataLength.ToString("X4");
                textBox8.Text = Tools.HexToString(TempMB.Data);
            }
            else
            {
                textBox6.Text = null;
                textBox7.Text = null;
                textBox8.Text = null;
            }
            textBox9.Text = TempMB.XorValue.ToString("X2");
            button3_Click(null, null);
            button4_Click(null, null);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string[] SPNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            for (int i = 0; i < SPNames.Length; i++)
            {
                comboBox1.Items.Add(SPNames[i]);
                pictureBox2.Image = Properties.Resources.Ok;
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
            if (button2.Text == "打开串口")
                try
                {

                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.Open();
                    button2.Text = "关闭串口";
                    button2.ForeColor = Color.White;
                    button2.BackColor = Color.Red;
                    pictureBox1.Image = Properties.Resources.Open;
                    groupBox2.Enabled = true;
                    button4.Enabled = true;
                }
                catch (Exception E) { MessageBox.Show("端口不存在或被占用", "警告"); }
            else
                try
                {
                    button2.Text = "打开串口";
                    button2.ForeColor = button1.ForeColor;
                    button2.BackColor = button1.BackColor;
                    serialPort1.Close();
                    pictureBox1.Image = Properties.Resources.Close;
                    groupBox2.Enabled = false;
                    button4.Enabled = false;
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
        }
        private float GetValue(byte OffSet)
        {
            MB TempMB = new MB();
            byte[] Buf = new byte[100];
            ModBus.TargetAddr = ModBusClass.TargetAddr;
            ModBus.HostAddr = ModBusClass.HostAddr;
            ModBus.MsgFlag = 0x13;
            ModBus.MsgNum = 1;
            ModBus.DataLength = 0;
            ModBus.DataFlag = OffSet;
            ModBus.MsgLength = ModBus.DataLength + 3;
            ModBus.Data = Tools.StringToHex(textBox8.Text);
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            SetBoxFromStruct(ModBus);
            button3_Click(null, null);
            button4_Click(null, null);

            ModBusClass.ModBus_CreatStruct(ref TempMB, Tools.StringToHex(textBox11.Text));
            float TempF = Tools.ByteToFloat(TempMB.Data, 0, 1);
            byte[] Fbuf = new byte[4];
            return TempF;
        }
        private void SetValue(byte OffSet, float F)
        {
            byte[] Buf = new byte[100];
            ModBus.TargetAddr = ModBusClass.TargetAddr;
            ModBus.HostAddr = ModBusClass.HostAddr;
            ModBus.MsgFlag = 0x05;
            ModBus.MsgNum = 1;
            ModBus.DataFlag = OffSet;
            ModBus.DataLength = 4;
            ModBus.MsgLength = ModBus.DataLength + 3;
            ModBus.Data = new byte[4];
            Tools.FloatToBytes(F, ref ModBus.Data);
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            SetBoxFromStruct(ModBus);
            button3_Click(null, null);
            button4_Click(null, null);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[100];
            GetBoxToStruct(ref ModBus);
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);//根据结构体获取buf
            ModBus.XorValue = Tools.Xor(Buf, ModBus.MsgLength + 7);//计算bufxor校验
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            textBox9.Text = ModBus.XorValue.ToString("X2");
            textBox10.Text = Tools.HexToString(Buf);
        }
        private void SetBar(int Value, int Max)
        {
            toolStripProgressBar1.Maximum = Max;
            toolStripProgressBar1.Value = Value;
            toolStripStatusLabel1.Text = (Value * 100 / Max).ToString() + "%";
            Application.DoEvents();
        }
        private void ReadStructFromBuffer()
        {
            int n = serialPort1.BytesToRead;
            byte[] Buffer = new byte[1000];
            serialPort1.Read(Buffer, 0, n);
            //ModBusClass.ModBus_CreatStruct(ref RXModBus, Buffer);
        }
        private void button4_Click(object sender, EventArgs e)
        {
           // SetBar(0, 100);
            byte[] RxBuffer = new byte[100];
            textBox11.Text = null;
            GetBoxToStruct(ref ModBus);

            byte[] Buf = new byte[ModBus.MsgLength + 9];
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            bool R = Usart.SendData(serialPort1, Buf, Buf.Length, ref RxBuffer, 100);
            if (ModBus.TargetAddr != ModBusClass.BroadAddr & R) textBox11.Text = Tools.HexToString(RxBuffer);
            ModBusClass.ModBus_CreatStruct(ref ModBus, RxBuffer);
        }
        

        private void button7_Click(object sender, EventArgs e)
        {
            byte B=byte.Parse(textBox14.Text);
           textBox15.Text=GetValue(B).ToString();
        }

       
        private void button8_Click(object sender, EventArgs e)
        {
            byte B = byte.Parse(textBox12.Text);
            float F = float.Parse(textBox13.Text);
            SetValue(B, F);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ValueSize; i++)
            {
                byte B =byte.Parse(dataGridView1[0,i].Value.ToString());
                dataGridView1[3,i].Value = GetValue(B).ToString();
                SetBar(i, ValueSize-1);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ValueSize; i++)
            {
                SetValue(byte.Parse(dataGridView1[0, i].Value.ToString()),float.Parse(dataGridView1[2, i].Value.ToString()));
                SetBar(i, ValueSize-1);
            }
        }
        private void SetDataGridFromStruct(Configs CFG)
        {
            int i = 0;
            dataGridView1[2, i++].Value =CFG.F_H_Value;
            dataGridView1[2, i++].Value =CFG.F_C_Value;
            dataGridView1[2, i++].Value =CFG.T_C_Value;
            dataGridView1[2, i++].Value =CFG.T_T_Value;
            dataGridView1[2, i++].Value = CFG.SH2_Value;
            dataGridView1[2, i++].Value = CFG._T_Value;
            dataGridView1[2, i++].Value = CFG.SU_Value;
            dataGridView1[2, i++].Value = CFG.TOP_Value;
            dataGridView1[2, i++].Value = CFG.RS485_Addr;
            dataGridView1[2, i++].Value = CFG.Baud_Value;
        }
        private void button13_Click(object sender, EventArgs e)
        {
            Configs CFG=new Configs();
            App.SetDefValue(ref CFG);
            SetDataGridFromStruct(CFG);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[100];
            ModBus.TargetAddr = ModBusClass.TargetAddr;
            ModBus.HostAddr = ModBusClass.HostAddr;
            ModBus.MsgFlag = 0x85;
            ModBus.MsgNum = 1;
            ModBus.DataFlag = 0x01;
            ModBus.DataLength = 0;
            ModBus.MsgLength = ModBus.DataLength + 3;
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            SetBoxFromStruct(ModBus);
            button3_Click(null, null);
            button4_Click(null, null);
            if (ModBus.Data[0] == 0x01) MessageBox.Show("保存成功");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            byte[] Buf = new byte[100];
            ModBus.TargetAddr = ModBusClass.TargetAddr;
            ModBus.HostAddr = ModBusClass.HostAddr;
            ModBus.MsgFlag = 0x86;
            ModBus.MsgNum = 1;
            ModBus.DataFlag = 0x01;
            ModBus.DataLength = 0;
            ModBus.MsgLength = ModBus.DataLength + 3;
            ModBusClass.ModBus_CreatBuf(ref ModBus, ref Buf);
            SetBoxFromStruct(ModBus);
            button3_Click(null, null);
            button4_Click(null, null);
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string fileName = (e.Data.GetData(DataFormats.FileDrop, false) as String[])[0];
            try
            {
                this.pictureBox1.ImageLocation = fileName;
                MessageBox.Show(fileName);
            }
            catch (Exception) { MessageBox.Show("文件格式不对"); }
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.All;

            else e.Effect = DragDropEffects.None; 
        }

        private void button14_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ValueSize; i++) dataGridView1[2, i].Value = dataGridView1[3, i].Value;
        }


    }
}
