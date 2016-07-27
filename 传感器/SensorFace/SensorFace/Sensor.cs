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

namespace SensorFace
{
    public partial class Sensor : Form
    {
        public Sensor()
        {
            InitializeComponent();
        }
        Bitmap BMP_H,BMP_M,BMP_D,BMP_high,BMP_B;
        private int KeyMode = 0;
        private const int KFCC_Start = 1, KFCC_Stop = 2, KFCC_UDOpen = 3, KFCC_UDClose = 4, KFCC_DDOpen = 5, KFCC_DDClose = 6,
        KFCC_TPowerON = 7, KFCC_TPowerOFF = 8, KFCC_Forward1 = 9, KFCC_Forward2 = 10, KFCC_Forward3 = 11, KFCC_Back1 = 12, KFCC_Back2 = 13,
        KFCC_Back3 = 14,KFCC_Reset=15, KFCC_Jerk=16;
         Fiter AngleFiter;
        float AngleValue;
        byte[] TxBuffer = new byte[100];
        byte[] RxBuffer = new byte[100];
        int TxBufferSize;
        MB TxModBus = new MB();
        MB RxModBus = new MB();
        private void Sensor_Load(object sender, EventArgs e)//加载窗口
        {
            button1_Click(null, null);
            button2_Click(null, null);
            BMP_M = new Bitmap(Properties.Resources.system_s);
            BMP_H = new Bitmap(Properties.Resources.system_m);
            BMP_D = new Bitmap(Properties.Resources.system_dot);
            BMP_high = new Bitmap(Properties.Resources.system_highlights);
            BMP_B = new Bitmap(Properties.Resources.system);
            AngleFiter = new Fiter(30);
            tabControl1.SelectedTab.BackColor = groupBox2.BackColor;
            OpenFileDialog Dlg = new OpenFileDialog();
            Read_UpDataInfor();
        }
        private string Read_UpDataInfor()
        {
            try
            {
                string name=Ini.Read("附件路径");
                textBox16.Text = name;
                FileInfo FI = new FileInfo(name);
                textBox19.Text = FI.Length.ToString();
                return name;
            }
            catch { }
            return "NULL";
        }
        private void button1_Click(object sender, EventArgs e)//扫描串口
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

        private void button2_Click(object sender, EventArgs e)//打开串口
        {
            if (button2.Text == "打开串口")
                try
                {

                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);
                    serialPort1.Open();
                    Value.App_Run = true ;
                    button2.Text = "关闭串口";
                    button2.ForeColor = Color.Red;
                    pictureBox3.Image = Properties.Resources.Open;
                }
                catch { MessageBox.Show("端口不存在或被占用", "警告"); }
            else
                try
                {
                    button2.Text = "打开串口";
                    Value.App_Run = false;
                    button2.ForeColor = button1.ForeColor;
                    serialPort1.Close();
                    pictureBox3.Image = Properties.Resources.Close;
                }
                catch(Exception E) { MessageBox.Show(E.Message); }
        }
      
        
        private void button8_Click(object sender, EventArgs e)//回环测试
        {
            byte[] TXBuffer=System.Text.Encoding.ASCII.GetBytes (textBox3.Text);
            byte[] RXBuffer=new byte[100];
            pictureBox1.Image = Properties.Resources.Close;
            textBox4.Text = null;
            Application.DoEvents();
            int Length=Usart.SendDataOne(serialPort1, TXBuffer,TXBuffer.Length, ref RXBuffer, 100);
            string String = System.Text.Encoding.ASCII.GetString(RXBuffer);
            if (Length > 0) pictureBox1.Image = Properties.Resources.Open;
            textBox4.Text=String;
            
        }
      
        private void FillValue()
        {
            ModBusClass.ModBus_Clear(ref TxModBus);//ModBusClass.HostAddr
            ModBusClass.ModBus_Clear(ref RxModBus);
            ModBusClass.ModBus_Create(ref ModBusClass.DefMoBus, 2, byte.Parse(comboBox3.Text), MasterSlaveMode.WorkMode_Master, ModBusClass.CheakMode_Crc);//产生默认配置
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
            int RxLength = Usart.SendData(serialPort1, TxBuffer, TxLength, ref RxBuffer, 100);
           // MessageBox.Show(Tools.HexToString(RxBuffer, 100));
            textBox6.Text = Tools.HexToString(RxBuffer, RxLength);
            ModBusClass.ModBus_Expend(RxBuffer, RxLength, ref RxModBus);
            if (RxModBus.ErrorFlag == ModBusClass.ModBus_Ok)
            {
                string R = Tools.GetStringFromByte(RxModBus.Data);
                if (Mode == ACFF.ACFF_GetCPUModel) label16.Text = R;
                if (Mode == ACFF.ACFF_GetBSD) label17.Text = R;
                if (Mode == ACFF.ACFF_GetCPUID) label15.Text = R;
                if (Mode == ACFF.ACFF_GetCANSpeed) label14.Text = R;
                if (Mode == ACFF.ACFF_GetSensorModel) label10.Text = R;
                if (Mode == ACFF.ACFF_GetSensorNumber) label11.Text = R;
                if (Mode == ACFF.ACFF_GetSensorName) label12.Text = R;
                if (Mode == ACFF.ACFF_GetSensorNote) label13.Text = R;
                timer1.Enabled = true;
            }
            else if (TxModBus.SlaveAddr != ModBusClass.BroadAddr) textBox6.Text = "ModBus解析错误代码[" + RxModBus.ErrorFlag + "]";
            return Tools.GetStringFromByte(RxModBus.Data);
        }
        private string GetSensorHandParameter(ACFF Mode)
        {
            byte[] Data=new byte[100];
            return SendReadSensor(Mode, Data, 0);
        }
        private void label2_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetCPUModel);
        } 

        private void label1_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetBSD);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetCPUID);
        }

        private void label5_Click(object sender, EventArgs e)
        {
           GetSensorHandParameter(ACFF.ACFF_GetCANSpeed);
        }
        private void button3_Click(object sender, EventArgs e)//获取硬件参数
        {
            Usart.Delay = 20;
            for (int i = 1; i < 9; i++) 
            {
                GetSensorHandParameter((ACFF)i); button3.Text = i.ToString() + "/" + "8"; Application.DoEvents();
                progressBar1.Maximum = 8;
                progressBar1.Value = i;
            }
            progressBar1.Value = 0;
            button3.Text = "获取";
            Usart.Delay = 50;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetSensorModel);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetSensorNumber);
        }

        private void label8_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetSensorName);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetSensorNote);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string R=GetSensorHandParameter(ACFF.ACFF_GetSensorTest);
            if (R != "NULL")
            {
                pictureBox4.Image = Properties.Resources.Open;
                MessageBox.Show("Data=" + R, "通信成功!");
            }
            else
            {
                pictureBox4.Image = Properties.Resources.Close;
                MessageBox.Show("Data=" + R, "通信失败!");

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetAngleADC);
            textBox21.Text = Tools.ByteToInt16(RxModBus.Data, 0,1).ToString();
        }

        

        private void button11_Click(object sender, EventArgs e)
        {
            byte[] Data=new byte[100];
            Tools.ByteFromFloat(float.Parse(textBox9.Text.Trim()),ref Data,0,0);
            MessageBox.Show(Tools.HexToString(Data,4));
            string Msg=SendReadSensor(ACFF.ACFF_SetAngleMax, Data, 4);
            MessageBox.Show(Msg,"非常好");
        }
        private void button12_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.ACFF_GetAngle);
            textBox8.Text = Tools.ByteToFloat(RxModBus.Data, 0, 0).ToString();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[100];
            Tools.ByteFromFloat(float.Parse(textBox10.Text.Trim()), ref Data, 0, 0);
            string Msg = SendReadSensor(ACFF.ACFF_SetAngleMin, Data, 4);
            MessageBox.Show(Msg, "非常好");
        }
        private void ShowAngle(double A)//角度盘显示
        {
            try
            {
                Bitmap BGI = new Bitmap(BMP_B.Width, BMP_B.Height);
                Graphics g = Graphics.FromImage(BGI);
                g.DrawImage(BMP_B, 0, 0, BMP_B.Width, BMP_B.Height);
                g.TranslateTransform((float)BMP_B.Width / 2.0f, (float)BMP_B.Height / 2.0f);//移动中心位置

                int z = (int)A;
                float PY = 90;
                g.RotateTransform((float)((A - z) * 360) - PY);
                g.DrawImage(BMP_M, -BMP_M.Width / 2, -BMP_M.Height / 2, BMP_M.Width, BMP_M.Height);
                g.RotateTransform(PY - (float)(A - z) * 360);

                g.RotateTransform((float)A - PY);
                g.DrawImage(BMP_H, -BMP_H.Width / 2, -BMP_H.Height / 2, BMP_H.Width, BMP_H.Height);
                g.RotateTransform(PY - (float)A);
                g.DrawImage(BMP_D, -BMP_D.Width / 2, -BMP_D.Height / 2, BMP_D.Width, BMP_D.Height);
                g.TranslateTransform(-(float)pictureBox5.Width / 2.0f, -(float)pictureBox5.Height / 2.0f);//恢复原点

                g.DrawImage(BMP_high, 0, 0, pictureBox5.Width, pictureBox5.Height);
                pictureBox5.Image = BGI;
            }
            catch { }
        }
        private void GetAngle()//获取角度  线程存放在变量AngleVlue
        {
            while (Value.App_Run)
            {
                int RxLength = Usart.SendData(serialPort1, Usart.ThreadTxBuffer, Usart.ThreadTxBufferSize, ref Usart.ThreadRxBuffer, 100);
                ModBusClass.ModBus_Expend(Usart.ThreadRxBuffer, RxLength, ref Usart.ThreadRxModBusMsg);
                AngleValue = Tools.ByteToFloat(Usart.ThreadRxModBusMsg.Data, 0, 0);
                Thread.Sleep(100);
            }
        }
        private void button14_Click(object sender, EventArgs e)//打开关闭实时显示角度
        {
            if (button14.Text == "跟踪")
            {
                timer2.Enabled = true;
                button14.Text = "取消";
                GetSensorHandParameter(ACFF.ACFF_GetAngle);
                Array.Copy(TxBuffer, Usart.ThreadTxBuffer, 100);
                Usart.ThreadTxBufferSize = TxBufferSize;
                AngleValue=Tools.ByteToFloat(RxModBus.Data, 0, 0);
                if (!serialPort1.IsOpen) { Value.App_Run = false; return; }
                else Value.App_Run = true;
                Thread ServiceThread = new Thread(new ThreadStart(GetAngle));
                ServiceThread.Start();
            }
            else
            {
                timer2.Enabled = false;
                button14.Text = "跟踪";
                Value.App_Run = false;
            }
        }
        private void timer2_Tick(object sender, EventArgs e)//实时显示角度刷新定时器
        {
            ShowAngle(AngleFiter.FlowPoolFilter(AngleValue));
            textBox11.Text = AngleFiter.FlowPoolFilter(AngleValue).ToString("0.00");
            if (Value.App_Run == false) button14_Click(null, null);
        }
        private void Sensor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Value.App_Run = false;
            if (Usart.Busy)
            {
                MessageBox.Show("通信忙不能直接退出!", "警告!");
            }
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

        private void button41_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = Read_UpDataInfor();
            Dlg.Filter = "bin文件(*.bin)|*.bin|All文件(*.*)|*.*";
            if (Dlg.ShowDialog() == DialogResult.Cancel) return;
            Ini.Write("附件路径", Dlg.FileName);
            Read_UpDataInfor();
        }
        private void DeBug(string Text)
        {
            richTextBox1.SelectionColor= Color.FromArgb(new Random().Next(0, 200 * 200 * 200));
            richTextBox1.AppendText(Text+"\n");
        }
        private void button42_Click(object sender, EventArgs e)
        {
            string R = GetSensorHandParameter(ACFF.ACFF_GetSensorTest);
            DeBug("检测设备:"+R);
            byte[] Data = new byte[10]; ;
            string Text =textBox17.Text.Replace("0x","");
            Tools.ByteFromU32(Convert.ToUInt32(Text, 16),ref Data,0,0);
            string Msg = SendReadSensor(ACFF.SCFF_SetWriteAddr, Data, 4);
            DeBug("写入指针:"+Tools.HexToString(Data,4)+Msg);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[10];
            Data[0] = byte.Parse(comboBox4.Text);
            string Msg = SendReadSensor(ACFF.SCFF_SetChnenel, Data, 1);
            MessageBox.Show("返回<" + Msg + ">", "非常好");
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
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if(serialPort1.BytesToRead>0)
                if (!Usart.Busy)textBox12.Text=Tools.TryToString(serialPort1.ReadLine());
            }
            catch { }
        }

        private void button10_Click(object sender, EventArgs e)//打开关闭调试模式
        {
            byte[] Data = new byte[100];
            if (button10.Text == "调试模式")
            {
                string Msg = SendReadSensor(ACFF.ACFF_InDeBug, Data, 0);
                if (Msg == "Ok!")
                {
                    button10.Text = "工作模式";
                    button10.ForeColor = Color.Red;
                    pictureBox6.Image = Properties.Resources.Open;
                }

            }
            else
            {
                string Msg = SendReadSensor(ACFF.ACFF_OutDeBug, Data, 0);
                if (Msg == "Ok!")
                {
                    button10.Text = "调试模式";
                    button10.ForeColor = button11.ForeColor;
                    pictureBox6.Image = Properties.Resources.Close;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GetSensorHandParameter(ACFF.SCFF_GetCANStdId);
            textBox1.Text = Tools.ByteToInt16(RxModBus.Data, 0, 1).ToString();
            GetSensorHandParameter(ACFF.SCFF_GetCANGroup);
            textBox2.Text = Tools.ByteToInt16(RxModBus.Data, 0, 1).ToString();
        }


    }
}

