using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Net;
namespace SensorFace
{
    public partial class Sensor : Form
    {
        public Sensor()
        {
            InitializeComponent();
        }
        DrawWatch DrawHumi;
        DrawWatch DrawTemp;
        DrawWatch DrawVol;
        DrawWatch DrawUserate_CPU;

        private int KeyMode = 0;
        private const int KFCC_Start = 1, KFCC_Stop = 2, KFCC_UDOpen = 3, KFCC_UDClose = 4, KFCC_DDOpen = 5, KFCC_DDClose = 6,
        KFCC_TPowerON = 7, KFCC_TPowerOFF = 8, KFCC_Forward1 = 9, KFCC_Forward2 = 10, KFCC_Forward3 = 11, KFCC_Back1 = 12, KFCC_Back2 = 13,
        KFCC_Back3 = 14,KFCC_Reset=15, KFCC_Jerk=16;
        float AngleValue;
        float HumiValue;
        float Vol;
        byte Userate_CPU;
        byte[] TxBuffer = new byte[100];
        byte[] RxBuffer = new byte[100];
        int TxBufferSize;
        MB TxModBus = new MB();
        MB RxModBus = new MB();
        private void ResetCom()
        {
            try
            {
                Value.serialPort1.Close();
            }
            catch { }
            try
            {
                Value.serialPort1 = new System.IO.Ports.SerialPort();
                Value.serialPort1.PortName = Ini.Read("COM");
                Value.serialPort1.BaudRate = 115200;
                Value.serialPort1.Open();
            }
            catch { (new SetPort()).ShowDialog(); if (FailCount++ > 2) return; ResetCom(); }
        }
        private void Sensor_Load(object sender, EventArgs e)//加载窗口
        {
            int i;
            Value.App_Run = true;
            tabControl1.TabPages.RemoveAt(2);
            if (int.TryParse(Ini.Read("StartTabIndex"), out i))
            {
                tabControl1.SelectedIndex = i;
            }
            tabControl1.SelectedTab.BackColor = groupBox2.BackColor;
            OpenFileDialog Dlg = new OpenFileDialog();
            ResetCom();
            DrawHumi = new DrawWatch();
            DrawTemp = new DrawWatch();
            DrawVol = new DrawWatch();
            DrawUserate_CPU = new DrawWatch();
            DrawHumi.Init();
            DrawTemp.Init();
            DrawVol.Init();
            DrawUserate_CPU.Init();
            ServiceThread = new Thread(new ThreadStart(GetAngle));
        }


      
        
        private void button8_Click(object sender, EventArgs e)//回环测试
        {
            byte[] TXBuffer=System.Text.Encoding.GetEncoding("GB2312").GetBytes (textBox3.Text);
            byte[] RXBuffer=new byte[100];
            textBox4.Text = null;
            Application.DoEvents();
            int Length=Usart.SendDataOne(Value.serialPort1, TXBuffer,TXBuffer.Length, ref RXBuffer, 100);
            string String = System.Text.Encoding.GetEncoding("GB2312").GetString(RXBuffer);
            textBox4.Text=String;
            
        }
      
        private void FillValue()
        {
            ModBusClass.ModBus_Clear(ref TxModBus);//ModBusClass.HostAddr
            ModBusClass.ModBus_Clear(ref RxModBus);
            ModBusClass.ModBus_Create(ref ModBusClass.DefMoBus, 2, byte.Parse("5"), MasterSlaveMode.WorkMode_Master, ModBusClass.CheakMode_Crc);//产生默认配置
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref TxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref RxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadRxModBusMsg);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadTxModBusMsg);

        }
        private string SendReadSensor(ACFF Mode,byte[] Data,int Length)
        {
            Random ra = new Random();
            timer1.Enabled = false;
            FillValue();
            TxModBus.MsgFlag = (byte)Mode;
            TxModBus.DataFlag = 0x01;
            TxModBus.DataLength = Length;
            TxModBus.MsgLength = Length+3;
            int TxLength = ModBusClass.ModBus_CreateMsg(ref TxBuffer, ref TxModBus, (int)Mode, ra.Next(0, 255), 0x91, Data,Length);
            textBox5.Text = Tools.HexToString(TxBuffer, TxLength);
            TxBufferSize = TxLength;
            RxBuffer = new byte[512];
            int RxLength = Usart.SendData(Value.serialPort1, TxBuffer, TxLength, ref RxBuffer, 100);
           // MessageBox.Show(Tools.HexToString(RxBuffer, 100));
            textBox6.Text = Tools.HexToString(RxBuffer, RxLength);
            ModBusClass.ModBus_Expend(RxBuffer, RxLength, ref RxModBus);
            if (RxModBus.ErrorFlag == ModBusClass.ModBus_Ok)
            {
                string R = Tools.GetStringFromByte(RxModBus.Data);
                if (Mode == ACFF.SCFF_GetSystemInfor) label16.Text = R;
                if (Mode == ACFF.SCFF_GetCPUModel) label17.Text = R;
                if (Mode == ACFF.SCFF_GetHandVersion) label15.Text = R;
                if (Mode == ACFF.SCFF_GetSoftVersion) label14.Text = R;
                if (Mode == ACFF.SCFF_GetSensorModel) label10.Text = R;
                if (Mode == ACFF.SCFF_GetSensorNumber) label11.Text = R;
                if (Mode == ACFF.SCFF_GetSensorName) label12.Text = R;
                if (Mode == ACFF.SCFF_GetSensorNote) label13.Text = R;
                if (Mode == ACFF.SCFF_GetProgTime) label22.Text = R;
                if (Mode == ACFF.SCFF_GetFlashSize) label41.Text = R;
                if (Mode == ACFF.SCFF_GetLibVersion) label43.Text = R;
            }
            else if (TxModBus.SlaveAddr != ModBusClass.BroadAddr) textBox6.Text = "ModBus解析错误代码[" + RxModBus.ErrorFlag + "]";
            return Tools.GetStringFromByte(RxModBus.Data);
        }
        private string SendRead(ACFF Mode, byte[] Data, int Length)
        {
            try
            {
                Random ra = new Random();
                FillValue();
                TxModBus.MsgFlag = (byte)Mode;
                TxModBus.DataFlag = 0x01;
                TxModBus.DataLength = Length;
                TxModBus.MsgLength = Length + 3;
                int TxLength = ModBusClass.ModBus_CreateMsg(ref TxBuffer, ref TxModBus, (int)Mode, ra.Next(0, 255), 0x91, Data, Length);
                TxBufferSize = TxLength;
                int RxLength = Usart.SendData(Value.serialPort1, TxBuffer, TxLength, ref RxBuffer, 100);
                ModBusClass.ModBus_Expend(RxBuffer, RxLength, ref RxModBus);
                return Tools.GetStringFromByte(RxModBus.Data);
            }
            catch { try { Value.serialPort1.Open(); } catch { }; return "NULL"; }
        }
        private string GetSensorHandParameter(ACFF Mode)
        {
            byte[] Data=new byte[100];
            return SendReadSensor(Mode, Data, 0);
        }
        private void label2_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetSystemInfor);
        } 

        private void label1_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetCPUModel);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetHandVersion);
        }

        private void label5_Click(object sender, EventArgs e)
        {
           GetSensorHandParameter(ACFF.SCFF_GetSoftVersion);
        }
        private void button3_Click(object sender, EventArgs e)//获取硬件参数
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            try { ServiceThread.Abort(); }
            catch { }
            Usart.Delay = 20;
            for (int i = 1; i < 12; i++) 
            {
                GetSensorHandParameter((ACFF)i); button3.Text = i.ToString() + "/" + "8"; Application.DoEvents();
            }
            button3.Text = "获取";
            Usart.Delay = 50;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetSensorModel);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetSensorNumber);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetSensorName);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetSensorNote);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string R=GetSensorHandParameter(ACFF.SCFF_GetSensorTest);
            if (R != "NULL")
            {
                MessageBox.Show("Data=" + R, "通信成功!");
            }
            else
            {
                MessageBox.Show("Data=" + R, "通信失败!");

            }
        }


        

        private void GetAngle()//获取角度  线程存放在变量AngleVlue
        {
            while (Value.App_Run)
            {
                int RxLength = Usart.SendData(Value.serialPort1, Usart.ThreadTxBuffer, Usart.ThreadTxBufferSize, ref Usart.ThreadRxBuffer, 100);
                ModBusClass.ModBus_Expend(Usart.ThreadRxBuffer, RxLength, ref Usart.ThreadRxModBusMsg);
                AngleValue = Tools.ByteToFloat(Usart.ThreadRxModBusMsg.Data, 0, 0);
                HumiValue = Tools.ByteToFloat(Usart.ThreadRxModBusMsg.Data, 4, 0);
                
                try
                {
                    Vol = Tools.ByteToFloat(Usart.ThreadRxModBusMsg.Data, 8, 0);
                    Userate_CPU = Usart.ThreadRxModBusMsg.Data[12];
                }
                catch { }
                Thread.Sleep(100);
            }
        }
        Thread ServiceThread;
        private void button14_Click(object sender, EventArgs e)//打开关闭实时显示角度
        {
            
            timer1.Enabled = false;
            if (button14.Text == "跟踪")
            {
                timer2.Enabled = true;
                button14.Text = "取消";
                GetSensorHandParameter(ACFF.SCFF_GetSensorValue);
                Array.Copy(TxBuffer, Usart.ThreadTxBuffer, 100);
                Usart.ThreadTxBufferSize = TxBufferSize;
                AngleValue=Tools.ByteToFloat(RxModBus.Data, 0, 0);
                if (!Value.serialPort1.IsOpen) { Value.App_Run = false; return; }
                else Value.App_Run = true;
                ServiceThread = new Thread(new ThreadStart(GetAngle));
                ServiceThread.Start();
            }
            else
            {
                timer2.Enabled = false;
                button14.Text = "跟踪";
                Value.App_Run = false;
                ServiceThread.Abort();
            }
        }
        private void timer2_Tick(object sender, EventArgs e)//实时显示角度刷新定时器
        {
            pictureBox5.Image = DrawTemp.Create(DrawTemp.F.FlowPoolFilter(AngleValue), pictureBox5);
            pictureBox1.Image = DrawHumi.Create(DrawHumi.F.FlowPoolFilter(HumiValue), pictureBox1);
            pictureBox2.Image = DrawVol.Create(DrawVol.F.FlowPoolFilter(Vol), pictureBox2);
            pictureBox3.Image = DrawUserate_CPU.Create(DrawUserate_CPU.F.FlowPoolFilter(Userate_CPU), pictureBox2);
            textBox11.Text =AngleValue.ToString("0.00℃");
            textBox22.Text = HumiValue.ToString("0.00")+"%";
            textBox1.Text=Vol.ToString("0.00")+"V";
            textBox2.Text = Userate_CPU + "%";
            if (Value.App_Run == false) button14_Click(null, null);
        }
        private void Sensor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Value.App_Run = false;
            ServiceThread.Abort();
            if (Usart.Busy)
            {
                MessageBox.Show("通信忙不能直接退出!", "警告!");
            }
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[100];
            Data[0] = byte.Parse(textBox13.Text);
            string Msg = SendReadSensor(ACFF.SCFF_SetCTRBIT, Data, 1);
           // MessageBox.Show("返回<"+Msg+">", "非常好");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[100];
            Data[0] = byte.Parse(textBox13.Text);
            string Msg = SendReadSensor(ACFF.SCFF_ClrCTRBIT, Data, 1);
          //  MessageBox.Show("返回<" + Msg + ">", "非常好");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            byte[] Data;
            Data = Tools.StringToHex(textBox14.Text);
            string Msg = SendReadSensor(ACFF.SCFF_SetCTRUINT32, Data, 4);
           // MessageBox.Show("返回<" + Msg + ">", "非常好");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetSIGUINT32);
            textBox15.Text = Tools.HexToString(RxModBus.Data, RxModBus.DataLength);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillValue();
        }

        private void button20_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Forward1;
            timer3.Enabled = true;
        }

        private void button20_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }
        private void SendClickButton(byte i)
        {
            byte[] Data = new byte[100];
            Data[0] = i;
            SendReadSensor(ACFF.SCFF_ButtonClick, Data, 1);
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            SendClickButton((byte)KeyMode);
        }

        private void button31_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_UDOpen;
            timer3.Enabled = true;
        }

        private void button31_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button33_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode =KFCC_UDClose;
            timer3.Enabled = true;
        }

        private void button33_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button19_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Forward2;
            timer3.Enabled = true;
        }

        private void button19_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button27_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Forward3;
            timer3.Enabled = true;
        }

        private void button27_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button21_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Back1;
            timer3.Enabled = true;
        }

        private void button21_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button22_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Back2;
            timer3.Enabled = true;
        }

        private void button28_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Back3;
            timer3.Enabled = true;
        }

        private void button39_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Reset;
            timer3.Enabled = true;
        }

        private void button39_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button40_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Jerk;
            timer3.Enabled = true;
        }

        private void button40_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }
  
        private void button22_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button28_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button35_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_DDOpen;
            timer3.Enabled = true;
        }

        private void button35_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button34_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_DDClose;
            timer3.Enabled = true;
        }

        private void button34_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button36_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_TPowerON;
            timer3.Enabled = true;
        }

        private void button36_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void tabPage2_Leave(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            try
            {
                ServiceThread.Abort();
            }
            catch { }
            button14.Text = "跟踪";
        }

        private void button32_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_TPowerOFF;
            timer3.Enabled = true;
        }

        private void button32_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }
        private void button29_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Start;
            timer3.Enabled = true;
        }

        private void button29_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button30_MouseDown(object sender, MouseEventArgs e)
        {
            KeyMode = KFCC_Stop;
            timer3.Enabled = true;
        }

        private void button30_MouseUp(object sender, MouseEventArgs e)
        {
            timer3.Enabled = false;
        }

        private void button19_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedTab.BackColor = groupBox2.BackColor;
            Ini.Write("StartTabIndex", tabControl1.SelectedIndex.ToString());
        }
        int FailCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
           
            if(FailCount==0)
            try
            {
                if(Value.serialPort1.BytesToRead>0)
                    if (!Usart.Busy)
                    {
                        byte[] Buf=new byte[Value.serialPort1.BytesToRead];
                        Value.serialPort1.Read(Buf, 0, Value.serialPort1.BytesToRead);
                        richTextBox2.AppendText(Encoding.GetEncoding("GB2312").GetString(Buf));
                    }
                FailCount = 0;
            }
            catch { FailCount =20; }
            if(FailCount>0)FailCount--;
        }
 
        private void button45_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[100];
            Data[0] =byte.Parse(textBox23.Text);
            Data[1] =Tools.StringToHex(textBox24.Text)[0];
            string Msg = SendReadSensor(ACFF.SCFF_SetReg, Data, 2);
            textBox24.Text = ((byte)(~Data[1])).ToString("x2");
        }



        private void Sensor_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Value.serialPort1.Dispose();
            }
            catch { }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            byte[] Data = new byte[1];
            Data[0] =byte.Parse(comboBox5.Text.Substring(comboBox5.Text.IndexOf('=') + 1, comboBox5.Text.Length - comboBox5.Text.IndexOf('=')-1));
            string Msg = SendReadSensor(ACFF.SCFF_SetDeBugMode, Data, Data.Length);
            richTextBox2.AppendText(Msg);
            timer1.Enabled = true;
        }

        private void richTextBox2_Enter(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            timer1.Enabled = true;

        }

        private void SendSensor(ACFF Mode, byte[] Data, int Length)
        {
            Random ra = new Random();
            timer1.Enabled = false;
            FillValue();
            TxModBus.MsgFlag = (byte)Mode;
            TxModBus.DataFlag = 0x01;
            TxModBus.DataLength = Length;
            TxModBus.MsgLength = Length + 3;
            int TxLength = ModBusClass.ModBus_CreateMsg(ref TxBuffer, ref TxModBus, (int)Mode, ra.Next(0, 255), 0x91, Data, Length);
            textBox5.Text = Tools.HexToString(TxBuffer, TxLength);
            TxBufferSize = TxLength;
            Value.serialPort1.Write(TxBuffer, 0, TxLength);
        }
        private void button5_Click_1(object sender, EventArgs e)
        {
            richTextBox2.Text = null;
            timer2.Enabled = false;
            byte[] Data = new byte[0];
            SendSensor(ACFF.SCFF_PrintHistory, Data, Data.Length);
            timer1.Enabled = true;
            button10.Enabled = true;
            button41.Enabled = true;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            richTextBox2.Text = "发送清空指令，请耐心等待\r\n";
            timer2.Enabled = false;
            byte[] Data = new byte[0];
            SendSensor(ACFF.SCFF_CleanHistory, Data, Data.Length);
            timer1.Enabled = true;
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv files(*.csv)|*.csv|txt files(*.txt)|*.txt|All files(*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs1 = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1, Encoding.Default);
                sw.WriteLine(richTextBox2.Text);
                sw.Close();
                fs1.Close();
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            this.Hide();
            Chart form = new Chart(richTextBox2.Text);
            form.ShowDialog();
            this.Show();
        }

    }
}

