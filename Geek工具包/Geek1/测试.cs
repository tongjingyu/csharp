using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.IO;
namespace Geek1
{
    public partial class 手环测试 : Form
    {
        public 手环测试()
        {
            InitializeComponent();
        }
        Value.CheckInfor CITemp;
         Value.CheckInfor CIWrite;
        private void 手环测试_Load(object sender, EventArgs e)
        {
            Value.Run = true;
            button1.Enabled = false;
         //   LoginIDE.Create();
            CITemp = new Value.CheckInfor();
    
          
        }
        private void WriteY()
        {
            SendAT(Value.BLE初始化命令);
            Thread.Sleep(100);
            SendAT(Value.BLE获取参数1);
            Thread.Sleep(100);
            SendAT(Value.BLE获取参数2);
            Thread.Sleep(100);
            SendAT(Value.BLE获取参数3);
            Thread.Sleep(100);
            SendAT(Value.BLE获取参数4);
            Thread.Sleep(100);

        }
        void Port1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(40);
            int n = Value.Port1.BytesToRead;
            byte[] buf = new byte[n];
            Value.Port1.Read(buf, 0, n);
            Value.ReviceData.Add(buf);
            timer1.Enabled = true;
        }
        private void SendAT(byte[] Msg)
        {
            byte[] RxBuf = new byte[100];
            BeginInvoke(new MethodInvoker(delegate()
            {
                richTextBox2.SelectionColor = GetRandomColor();
               richTextBox2.AppendText(Tools.HexToString(Msg, Msg.Length) + "\r\n");
            }));
            try { Value.Port1.Open(); }
            catch {  }
            Value.Port1.Write(Msg, 0, Msg.Length);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendAT(new byte[] { 0x01, 0x04, 0xfe, 0x03, 0x03, 0x01, 0x00 });
        }
        public System.Drawing.Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;

            return System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);
        }
        private void 手环测试_FormClosed(object sender, FormClosedEventArgs e)
        {
            Value.Run = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            while (Value.ReviceData.Count > 0)
            {
                byte[] item = Value.ReviceData[0];
                richTextBox1.SelectionColor = GetRandomColor();
                if (Tools.Comment(item, Value.BLE搜索到设备))
                {
                    SendAT(Value.BLE取消搜索);
                    Value.WorkFlag = 2;
                    richTextBox1.AppendText("搜索到设备:Mac:"+Tools.HexToString(item, 8, 6)+"信号:"+item[14]+"\r\n");
                    if (item[14] > CITemp.RISS)
                    {
                        CITemp.RISS = item[14];
                        CITemp.MAC = Tools.HexToString(item, 8, 6);
                        Value.CurMAC = Tools.HexToString(item, 8, 6);
                    }
                }
                if (Tools.Comment(item, Value.BLE搜索响应)) {  richTextBox1.AppendText("响应:\r\n"); }
                if (Tools.Comment(item, Value.BLE初始化完毕)) richTextBox1.AppendText("初始化完毕:\r\n");
                if (Tools.Comment(item, Value.BLE初始化成功)) richTextBox1.AppendText("BLE初始化成功:\r\n");
                if (Tools.Comment(item, Value.BLE操作忙)) richTextBox1.AppendText("BLE操作忙:\r\n");
                if (Tools.Comment(item, Value.BLE已经执行该操作))  richTextBox1.AppendText("BLE已经执行该操作:\r\n");
                if (Tools.Comment(item, Value.BLE配对响应)) richTextBox1.AppendText("BLE配对响应:\r\n");
                if (Tools.Comment(item, Value.BLE配对成功)) { Value.WorkFlag = 3; richTextBox1.AppendText("BLE配对成功:\r\n"); }
                if (Tools.Comment(item, Value.BLE断开连接)) richTextBox1.AppendText("BLE断开连接:\r\n");
                if (Tools.Comment(item, Value.BLE拒绝访问)) richTextBox1.AppendText("BLE拒绝访问:\r\n");
                if (Tools.Comment(item, Value.BLE读取成功)) richTextBox1.AppendText("BLE读取成功:\r\n");
                if (Tools.Comment(item, Value.BLE操作完成)) richTextBox1.AppendText("BLE操作完成:\r\n");
                if (Tools.Comment(item, Value.BLE未指命令)) richTextBox1.AppendText("BLE未指命令:\r\n");
                if (Tools.Comment(item, Value.BLE初始化)) richTextBox1.AppendText("BLE初始化:\r\n");
                if (Tools.Comment(item, Value.BLE获取参数)) richTextBox1.AppendText("BLE获取参数:\r\n");
                if (Tools.Comment(item, Value.BLE获取参数完毕)) { Value.WorkFlag = 1; richTextBox1.AppendText("BLE获取参数完毕:\r\n"); }
                if (Tools.Comment(item, Value.BLE测试完毕)) { Value.WorkFlag = 5; richTextBox1.AppendText("BLE测试完毕:\r\n"); }
                if (Tools.Comment(item, Value.BLE返回ff01))
                {

                    if (item[2] == 0x19)
                    {
                        string 版本 = item[27].ToString() + "." + item[26].ToString() + "." + item[25].ToString() + "." + item[24].ToString();
                        richTextBox1.AppendText("BLE返回ff01:\r\n" + 版本);
                        CITemp.版本 = 版本;
                    }
                    richTextBox1.AppendText("BLE获取参数:\r\n");
                    Value.WorkFlag = 4;
                    
                }
                if (Tools.Comment(item, Value.BLE返回ff0c))
                {

                    richTextBox1.AppendText("BLE返回ff0c:\r\n");
                    Value.WorkFlag = 6;
                    CITemp.电量 = item[12].ToString();
                }
                if (Tools.Comment(item, Value.BLE返回ff03))
                {

                    string 版本 = item[12].ToString();
                    CITemp.Status = item[12];
                    richTextBox1.AppendText("BLE返回ff03\r\n");
                    Value.WorkFlag = 7;
                }
                if (Tools.Comment(item, Value.BLE获扫描结果))
                {
                    int CountDevice = item[Value.BLE获扫描结果.Length];
                    richTextBox1.AppendText("BLE获扫描结果:" + CountDevice + "\r\n");
                    for (int i = 0; i < CountDevice; i++)
                    {
                        byte[] newA = item.Skip(Value.BLE获扫描结果.Length + 3+i*8).Take(6).ToArray();
                        richTextBox1.AppendText(Tools.HexToString(newA, newA.Length) + "\r\n");
                    }if(CountDevice==0)
                    {
                       // autoscanf.Abort();
                        Value.WorkFlag = 99;
                    }else Value.WorkFlag = 2;
                }
                richTextBox1.AppendText(Tools.HexToString(item, item.Length) + "\r\n");
                Value.ReviceData.RemoveAt(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendAT(new byte[] { 0x01, 0x09, 0xFE, 0x09, 0x00, 0x00, 0x00 }.Concat(Tools.StringToHex1(Value.CurMAC)).ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Replace(" ", ",0x");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(WriteY);
            t.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        Thread autoscanf;
        private void button10_Click(object sender, EventArgs e)
        {
            if(button10.Text=="选择串口")
            while (true)
            {
                COM设置 form = new COM设置(true);
                form.ShowDialog();
                Value.Port1 = new SerialPort();
                Value.Port1.PortName = Ini.Read("COM_Name");
                Value.Port1.BaudRate = int.Parse(Ini.Read("COM_BaudRate"));
                this.Text = Value.Port1.PortName;
                try
                {
                    Value.Port1.Open();
                    Value.Port1.DataReceived += Port1_DataReceived;
                    button10.Text = "自动";
                    break;
                }
                catch (Exception E) { MessageBox.Show(E.Message); }
            }
            if(button10.Text=="自动")
            {
            button10.Text = "停止";
            autoscanf = new Thread(自动);
            autoscanf.Start();
            CITemp.RISS = 0;
            timer2.Enabled = true;
            panel6.BackColor = panel4.BackColor;
            panel7.BackColor = panel4.BackColor;
            panel9.BackColor = panel4.BackColor;
            panel8.BackColor = panel4.BackColor;
            panel10.BackColor = panel4.BackColor;
            }else
            {
                button10.Text = "自动";
                autoscanf.Abort();
            }
        }
        private int WhileDo(int Key,int Goto)
        {
            int i = 200; 
            BeginInvoke(new MethodInvoker(delegate()
            {
                progressBar1.Maximum = 7;
                progressBar1.Value = Key;
            }));
            while(Value.Run)
            {
                if (Key ==Value.WorkFlag) return 0;
                if (Key == 99) return Goto;
                Thread.Sleep(100);
                if (i-- < 1) return Goto;
            }
            return Goto;
        }
        private string GetTitle1String()
        {
            string Temp;
            Temp = "端口,";
            Temp += "时间,";
            Temp += "MAC,";
            Temp += "串号,";
            Temp += "电量,";
            Temp += "版本,";
            Temp += "FlashID,";
            Temp += "Flash,";
            Temp += "GsensorID,";
            Temp += "Gsensor自检,";
            Temp += "心率TIA,";
            Temp += "ADC,";
            Temp += "心率校验,";
            Temp += "充电状态";
            return Temp;
        }
        private string GetContextString(Value.CheckInfor CI)
        {
            string Temp;
            Temp = CI.COMName + ",";
            Temp += CI.Date + ",";
            Temp += CI.MAC + ",";
            Temp += CI.SN + ",";
            Temp += CI.电量 + ",";
            Temp += CI.版本 + ",";
            Temp += CI.FlashID + ",";
            Temp += CI.Flash + ",";
            Temp += CI.GsensorID + ",";
            Temp += CI.Gsensor自检 + ",";
            Temp += CI.心率TIA + ",";
            Temp += CI.ADC + ",";
            Temp += CI.心率校验 + ",";
            Temp += CI.充电状态;
            return Temp;
        }
        bool Frits = true;
        private void 自动()
        {
            int i = 0,z=0;
            if(Frits)
            { 
            button3_Click(null, null);//初始化
            z=WhileDo(1,0);
            Frits = false;
            }
            SecCount = 0;
            A1: button6_Click(null, null);//搜索
            z=WhileDo(2,2);
            if (z > 0) goto A1;
            Thread.Sleep(200);
            A2: button1_Click(null, null);//连接
            z=WhileDo(3,2);
            z = WhileDo(3, 2);
            if (z > 0) goto A2;
            Thread.Sleep(2000);
            SendAT(Value.BLE测试);
            z = WhileDo(5, 2);
            if (z > 0) goto A2;
            Thread.Sleep(5000);
            SendAT(Value.BLE获取内容ff01);
            Thread.Sleep(1000);
            SendAT(Value.BLE获取内容ff0c);
            Thread.Sleep(1000);
            SendAT(Value.BLE获取内容ff03);
            Thread.Sleep(1000);
            SendAT(Value.BLE断开设备);
            Value.CheckInfor CI = Create.CreateInfor(CITemp.MAC, CITemp.版本, CITemp.电量, CITemp.Status, "0");
            CIWrite=Create.CreateInfor(CITemp.MAC, CITemp.版本, CITemp.电量, CITemp.Status, "0");
            CIWrite.WriteOk = true;
            CI.RISS = CITemp.RISS;
            BeginInvoke(new MethodInvoker(delegate()
            {
                button10.Text = "自动";
                progressBar1.Maximum = 0;
                if (CI.Flash == "错误") panel6.BackColor = Color.Red; else panel6.BackColor = Color.Green;
                if (CI.GsensorID == "错误") panel7.BackColor = Color.Red; else panel7.BackColor = Color.Green;
                if (CI.Gsensor自检 == "错误") panel10.BackColor = Color.Red; else panel10.BackColor = Color.Green;
                if (CI.ADC == "错误") panel8.BackColor = Color.Red; else panel8.BackColor = Color.Green;
                if (CI.充电状态 == "连接") panel9.BackColor = Color.Red; else panel9.BackColor = Color.Green;
                label7.Text ="MAC:"+ CI.MAC;
                label8.Text = "电量:" + CI.电量;
                label9.Text = "信号:" + CI.RISS;
                button1.Enabled = true;
                timer2.Enabled =false;

            }));
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("限制版只能保存一项测试结果");
            if(textBox2.Text.Length<3){
                MessageBox.Show("请先录入SN串号");
                return;
            }
            CIWrite.Date = DateTime.Now.ToString();
            CIWrite.COMName = Value.Port1.PortName;
            CIWrite.SN = textBox2.Text;
            string FilePath=System.IO.Directory.GetCurrentDirectory()+"\\测试结果.csv";
           // if (!File.Exists(FilePath))
            {
                FileStream fs1 = new FileStream(FilePath, FileMode.Create, FileAccess.Write);//创建写入文件 
                StreamWriter sw = new StreamWriter(fs1,Encoding.Default);
                 sw.WriteLine(GetTitle1String());//开始写入值
                 if (CIWrite.WriteOk) sw.WriteLine(GetContextString(CIWrite));//开始写入值
                CIWrite.WriteOk = false;
                sw.Close();
                fs1.Close();
                MessageBox.Show("文件创建成功:" + FilePath);

            }
            //else
            //{
            //    FileStream fs1 = new FileStream(FilePath, FileMode.Append, FileAccess.Write);//创建写入文件 
            //    StreamWriter sw = new StreamWriter(fs1, Encoding.Default);
            //    if(CIWrite.WriteOk)sw.WriteLine(GetContextString(CIWrite));
            //    CIWrite.WriteOk=false;
            //    sw.Close();
            //    fs1.Close();
            //}
            //button1.Enabled = false;
        }
        int SecCount = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            label10.Text = SecCount++.ToString();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
         button1_Click(null, null);//连接
         Thread.Sleep(1000);
            SendAT(Value.BLE测试);
        }
    }
}
