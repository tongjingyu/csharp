using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Xml;



namespace DTU配置工具V1._0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private const string SysConfigFilePath = "SysConfig.xml";
        static SerialPort CommPort = new SerialPort();
        private SysConfig CurConfig = new SysConfig();
     

       
        #region 串口通信
        private bool CommToDevice(byte Cmd, UInt32 Data,ref UInt32 Value)
        {
            byte[] CmdBuffer = new byte[9];
            byte[] RxBuffer = new byte[20];
            int RetryTimes = 1;
            UInt32 T;
            CmdBuffer[0] = 0x5A;
            CmdBuffer[1] = 0xA5;
            CmdBuffer[2] = Cmd;
            CmdBuffer[3] = (byte)(Data >> 24);
            CmdBuffer[4] = (byte)(Data >> 16);
            CmdBuffer[5] = (byte)(Data >> 8);
            CmdBuffer[6] = (byte)(Data);
            CmdBuffer[7] = 0x3C;
            CmdBuffer[8] = 0xC3;
            int ReadByteSCount=0;
            int LeftByteSCount=0;
            int RxLength=9;
            for (int i = 0; i < RetryTimes; i++)
            {
                try
                {
                    CommPort.DiscardOutBuffer();
                    CommPort.DiscardInBuffer();
                    CommPort.ReadTimeout = 200;                                          //200ms超时
                    CommPort.Write(CmdBuffer, 0, 9);                                     //写命令                   
                    ReadByteSCount = 0;
                    LeftByteSCount = 9;
                    while (ReadByteSCount < RxLength)
                    {                                                                    //开始读取数据
                        ReadByteSCount += CommPort.Read(RxBuffer, ReadByteSCount, LeftByteSCount);
                        LeftByteSCount = RxLength - ReadByteSCount;
                    }
                    if (ReadByteSCount >= RxLength) RetryTimes = 1000;
                    else break;
                }
                catch (TimeoutException)                                                 //读取超时
                {

                }
                if (ReadByteSCount == 9)
                {
                    if ((RxBuffer[0] == 0x5A) && (RxBuffer[1] == 0xA5) && (RxBuffer[7] == 0x3C) && (RxBuffer[8] == 0xC3)) {
                        T = RxBuffer[3]; T <<= 8;
                        T |= RxBuffer[4]; T <<= 8;
                        T |= RxBuffer[5]; T <<= 8;
                        T |= RxBuffer[6];
                        Value = T;
                        if (RxBuffer[2] == 0) return true;
                        else return false;
                    }
                }
            }
            Value = 0;
            return false;
        }
        #endregion

        #region 串口刷新
        private void button40_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] SportName = SerialPort.GetPortNames();
            for (int i = 0; i < SportName.Length; i++)
            {
                comboBox1.Items.Add(SportName[SportName.Length - 1 - i]);
            }
            if (SportName.Length > 0) comboBox1.SelectedIndex = 0;
        }
        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {
            button40_Click(null, null);
            for (int i = 1; i <= 255; i++)
            {
                comboBox4.Items.Add(i.ToString());
            }
            comboBox4.SelectedIndex = 0;
            timer1.Tag = 2;
            //button13.Enabled = false;
            //button14.Enabled = false;
            toolStripStatusLabel7.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #region 打开串口
        private void button39_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (button39.Text == "打开端口")
                {
                    COMM_UpdateMsg("打开端口", 0, " ",1);
                    CurConfig.Config.Address = int.Parse(comboBox4.Text.Trim());
                    bool R = CommByRS232.InitCommPort(comboBox1.Text.Trim(), int.Parse(comboBox2.Text.Trim()), 1, ModBusProc.HostMode);
                    if (R == true)
                    {
                        button39.Text = "关端端口";
                        button39.ForeColor = Color.White;
                        button39.BackColor = Color.Red;
                        COMM_UpdateMsg("", 100, "打开成功!",1);
                        
                        
                    }
                    else
                    {
                        COMM_UpdateMsg("", 100, "打开失败!",0);
                    }
                }
                else
                {
                    COMM_UpdateMsg("关闭端口", 0, " ",1);
                    if (CommByRS232.CommPort.IsOpen) CommByRS232.CommPort.Close();
                    button39.Text = "打开端口";
                    button39.ForeColor = Color.Black;
                    button39.BackColor = Color.Gainsboro;
                    COMM_UpdateMsg("", 100, "关闭成功!",1);
                }
            }
            catch (Exception E)
            {
                COMM_UpdateMsg("端口操作", 0, "操作失败!",0);
                MessageBox.Show(E.Message);
            }
        }
        #endregion

        #region 通讯测试
        private void button56_Click(object sender, EventArgs e)
        {
          
            COMM_UpdateMsg("正在进行通讯测试……", 0, " ",1);
            byte[] Productor = new byte[30];
            int Length = 0;
            bool R = CommByRS232.ReadPacket(CurConfig.Config.Address, 2002, 2, ref Productor, ref Length);
            COMM_UpdateMsg("", 80, "",1);
            if (R == true)
            {               
                COMM_UpdateMsg("", 100, "成功!",1);
            }
            else
            {
                COMM_UpdateMsg("", 100, "失败!",0);
            }
        }
        #endregion

       
        #region 时间解码
        public static DateTime RTC_DecodeTime(int TimeData)
        {
            int T, K, B;
            int T16, K16;
            int Year, Month, Day, Hour, Min, Sec;
            K16 = TimeData % 86400;		        	        //分离时/分/秒   
            T16 = TimeData / 86400;     		            //分离年/月/日   
            Sec = K16 % 60;                  		//计算秒   
            K16 /= 60; Min = K16 % 60;               	//计算分   
            K16 /= 60; Hour = K16 % 24;             		//计算时
            //Week = (T16 + 6) % 7;							//根据总天数计算星期
            for (K16 = 0, T = 0; T < 150; T++)
            {  		            //按天累计年份   
                K16 = (T & 0x03) > 0 ? 365 : 366;       	        //统计闰年 		
                if (T16 < K16) { K16 -= 337; break; }    	    //计算当年二月份虚天数
                else T16 -= K16;                  		//减掉年份    
            }
            Year = T;                      			//计算得到相对年份   
            for (T = 1; T < 13; T++)
            {
                if (((T + (T >> 3)) & 0x01) > 0) B = 31;
                else B = 30;
                K = (T == 2) ? K16 : B;    //统计当月天数
                if (T16 >= K) T16 -= K;             			//减掉已经足够的月份		
                else break;
            }
            Month = T;                     			//计算得到当年中的月份   
            Day = T16 + 1;                       		//计算得到当年中的天   
            DateTime DT = new DateTime(Year + 2000, Month, Day, Hour, Min, Sec);
            return DT;
        }
        #endregion

        #region 时间编码
        public static int RTC_EncodeTime(DateTime DT)
        {
            int T, K;
            int T32, TimeData = 0;
            for (T = 0, K = 0; T < (DT.Year - 2000); T++, K = T & 0x03)
            {				//累加年份				
                TimeData += K > 0 ? 31536000 : 31622400;
            }
            for (T = 1; T < DT.Month; T++)
            {
                if (T != 2)
                {										//统计平月
                    T32 = (((T + (T >> 3)) & 0x01) > 0) ? 2678400 : 2592000;
                }
                else T32 = K > 0 ? 2419200 : 2505600;					//统计闰月
                TimeData += T32;
            }
            T32 = DT.Day - 1; TimeData += T32 * 86400;					//统计天
            T32 = DT.Hour; TimeData += T32 * 3600;					//统计时
            T32 = DT.Minute; TimeData += T32 * 60;					//统计分
            TimeData += DT.Second;								//统计秒
            return TimeData;
        }
        #endregion   


        //从包提取数据
        private int ModbusPacket_GetData(byte[] Packet, int Offset)
        {
            int Val;
            Val = Packet[Offset++];
            Val <<= 8;
            Val |= Packet[Offset];
            return Val;
        }

        //从数据包里提取字符串
        private string ModbusPacket_GetString(byte[] Packet,int Offset, int Length)
        {
            byte[] TempData = new byte[Length + 8];
            for (int i = 0; i < Length; i++) {
                TempData[i] = Packet[Offset + i];
            }
            if ((Length % 2) > 0) Length++;
            byte T;
            for (int i = 0; i < Length-1; i+=2) {
                T = TempData[i];
                TempData[i] = TempData[i + 1];
                TempData[i + 1] = T;
            }
            string _Datas = Encoding.UTF8.GetString(TempData);


            //string[] _ss = _Datas.Split('\0');
            //_Datas = _ss[0];

            //char[] _chars = _Datas.ToCharArray();
            //char singlechar;



            ////奇数的算法
            //for (int i = 0; i < _Datas.Length-1; i = i + 2)
            //{
            //    singlechar = _chars[i];
            //    _chars[i] = _chars[i + 1];
            //    _chars[i + 1] = singlechar;

            //}
            //_Datas = new string(_chars);
           
            
            return _Datas;
        }


        //字符串转为byte
        private byte[] ModbusPacket_GetByte(ref byte[] Packet, string _ss, int Offset, int Length)
        {
            byte[] _data = new byte[Length];
            for (int i = 0; i < Length; i++) _data[i] = 0;
            if (_ss != null && _ss.Length <= Length)
            {
                byte singleByte;
                byte[] _data2 = System.Text.Encoding.ASCII.GetBytes(_ss.ToCharArray());
                for (int i = 0; i < _data2.Length; i++) _data[i] = _data2[i];
                for (int i = 0; i < _data.Length; i += 2)
                {
                    singleByte = _data[i];
                    _data[i] = _data[i + 1];
                    _data[i + 1] = singleByte;
                }
                for (int i = 0; i < _data.Length; i++)
                {
                    Packet[Offset + i] = _data[i];
                }
            }

            return Packet;
        }



        //添加数据到包
        private void ModbusPacket_AddData32(ref byte[] Packet, int Offset, int Data)
        {
            Packet[Offset++] = (byte)((Data >> 8) & 0xFF);
            Packet[Offset++] = (byte)(Data & 0xFF);
            Packet[Offset++] = (byte)((Data >> 24) & 0xFF);
            Packet[Offset] = (byte)((Data >> 16) & 0xFF);
        }

        //从包提取32位数据
        private int ModbusPacket_GetData32(byte[] Packet, int Offset)
        {
            int Val;
            Val = Packet[Offset + 2]; Val <<= 8;
            Val |= Packet[Offset + 3]; Val <<= 8;
            Val |= Packet[Offset]; Val <<= 8;
            Val |= Packet[Offset + 1];
            return Val;
        }

        //从包提取32位数据A
        private int ModbusPacket_GetData32A(byte[] Packet, int Offset)
        {
            int Val;
            Val = Packet[Offset]; Val <<= 8;
            Val |= Packet[Offset + 1]; Val <<= 8;
            Val |= Packet[Offset + 2]; Val <<= 8;
            Val |= Packet[Offset + 3];
            return Val;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            UInt32 T = 0;
            bool R;
            timer1.Tag = 0;
            timer1.Stop();
            
            if (CommPort.IsOpen == false) return;
            toolStripStatusLabel1.Text = "正在进入调试模式……";
            R = CommToDevice(0x03, 0, ref T);
            if (R == false)
            {
                toolStripStatusLabel1.Text = "通讯错误!";
                timer1.Interval = 500;
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                toolStripStatusLabel1.Text = "进入成功!……";
                timer1.Stop();
                timer1.Tag = 2;
            }

        }


        #region 同步设备时间
        private void button9_Click(object sender, EventArgs e)
        {
            int T;
            bool R;

            COMM_UpdateMsg("设置参数到设备 [0x" + CurConfig.Config.Address.ToString("X2") + "]", 0, " ", 1);
            T = CheckTools.RTC_EncodeTime(DateTime.Now.AddSeconds(1));
            DateTime DT = RTC_DecodeTime(T);
            CurConfig.Config.ProductDate = DT;
            byte[] ConfigDatas = new byte[200];
            

            ModbusPacket_AddData32(ref ConfigDatas, 0, RTC_EncodeTime(CurConfig.Config.ProductDate));

            R = CommByRS232.WritePacket(CurConfig.Config.Address, 2004, 2, ConfigDatas, 4);

            if (R == false)
            {
                COMM_UpdateMsg("失败", 0, "同步错误!", 0);
            }
            else
            {

                COMM_UpdateMsg("", 100, "读取成功!", 1);
                MessageBox.Show("  同步成功!当前设备内部时间:" + DT.ToString(), "消息", MessageBoxButtons.OK, MessageBoxIcon.None);

            }
        }
        #endregion

        #region 读取设备时间
        private void button12_Click(object sender, EventArgs e)
        {
            int T = 0;
            int RxLength=9;
            byte[] ConfigDatas = new byte[200];
            bool R;
            COMM_UpdateMsg("从设备读取配置参数 [0x" + CurConfig.Config.Address.ToString("X2") + "]", 0, " ", 1);
            R = CommByRS232.ReadPacket(CurConfig.Config.Address, 2004, 2, ref ConfigDatas, ref RxLength);
            
            if (R == false)
            {
                COMM_UpdateMsg("失败", 0, "读取错误!", 0);
            }
            else
            {
                T = ModbusPacket_GetData32(ConfigDatas, 0);
                CurConfig.Config.ProductDate = RTC_DecodeTime(T);
                COMM_UpdateMsg("",100,"读取成功!",1);
                MessageBox.Show("当前设备内部时间: " + CurConfig.Config.ProductDate.ToString(), "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }
        #endregion

        #region 进入配置模式
        //private void button13_Click(object sender, EventArgs e)
        //{
        //    UInt32 T = 0;
        //    bool R;
        //    if ((int)(timer1.Tag) != 0)
        //    {
        //        timer1.Tag = 0;
        //        if (CommPort.IsOpen == false)
        //        {
        //            COMM_UpdateMsg("失败", 0, "通讯端口未打开!", 0);
        //            return;
        //        }
        //        COMM_UpdateMsg("正在进入调试模式……", 0, "", 1);
        //        R = CommToDevice(0x03, 0, ref T);
        //        if (R == false)
        //        {
        //            COMM_UpdateMsg("", 0, "通讯错误!", 0);
        //            timer1.Interval = 500;
        //            timer1.Enabled = true;
        //            timer1.Start();
        //        }
        //        else
        //        {
        //            COMM_UpdateMsg("", 100, "进入成功!……",1);
        //            timer1.Stop();
        //            timer1.Tag = 2;
        //        }
        //    }
        //    else
        //    {
        //        timer1.Tag = 2;
        //        timer1.Stop();
        //    }
        //}
        #endregion

        #region 编写状态栏
        private void COMM_UpdateMsg(string CmdStr, int Process, string Result,int _Status)
        {
            if (CmdStr != "")
            {
                toolStripStatusLabel1.Text = "命令：" + CmdStr;
            }
            else
            {
                toolStripStatusLabel1.Text = "命令：" + CmdStr;
            }
            if (Process <= 100)
            {
                toolStripProgressBar1.Value = Process;
                toolStripStatusLabel3.Text = Process.ToString() + "%";
            }
            if (Result != "")
            {
                toolStripStatusLabel5.Text = Result;
            }
            else
            {
                toolStripStatusLabel5.Text = "";
            }
            if (_Status > 0)
            {
                toolStripStatusLabel5.ForeColor = Color.Black;
                toolStripStatusLabel1.ForeColor = Color.Black ;
            }
            else
            {
                toolStripStatusLabel5.ForeColor = Color.Red;
                toolStripStatusLabel1.ForeColor = Color.Red;
            }
            Application.DoEvents();   
        }
        #endregion


        #region 退出配置模式
        //private void button14_Click(object sender, EventArgs e)
        //{
        //    UInt32 T = 0;//UInt32 值类型表示值介于 0 和 4,294,967,295 之间的无符号整数。 
        //    bool R;
        //    if (CommPort.IsOpen == false)
        //    {
        //        COMM_UpdateMsg("失败", 0, "通讯端口未打开!", 0);
        //        return; 
        //    }
        //    R = CommToDevice(0x04, 0, ref T);
        //    if (R == false)
        //    {
        //        COMM_UpdateMsg("", 0, "通讯错误!", 0);
        //        //MessageBox.Show("通讯错误!", "消息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    else
        //    {
        //        COMM_UpdateMsg("", 100, "操作成功!", 1);
        //        //MessageBox.Show("操作成功!", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
        //    }
        //}
        #endregion

        #region 加载默认参数到控件的函数
        private void LoadDefaultParams(SysConfig.ConfigParams Config)
        {

                tb3.Text = Config.BaseLevel.ToString();
                Increaselevel.Text = Config.IncreaseLevel.ToString();

                if (Config.InterTimeSet.ToString() == "24")
                    cb5.SelectedIndex = 4;
                if (Config.InterTimeSet.ToString() == "1")
                    cb5.SelectedIndex = 0;
                if (Config.InterTimeSet.ToString() == "2")
                    cb5.SelectedIndex = 1;
                if (Config.InterTimeSet.ToString() == "4")
                    cb5.SelectedIndex = 2;
                if (Config.InterTimeSet.ToString() == "8")
                    cb5.SelectedIndex = 3;
                
                tb1.Text = Config.DevNumber.ToString();
                tb2.Text = Config.PasswordCode.ToString();
                tb5.Text = Config.CompName.ToString();
                tb6.Text = Config.StationName.ToString();
                checkBox1.Checked = Config.WarnEnable;
                tb7.Text = Config.WarningLevel.ToString("F1");
                cb3.SelectedValue = Config.WarnInterval;
                cb4.SelectedValue = Config.TelCount;
                tbtel1.Text = Config.TelNum1.ToString();
                tbtel2.Text = Config.TelNum2.ToString();
                tbtel3.Text = Config.TelNum3.ToString();
                tbtel4.Text = Config.TelNum4.ToString();
                tbtel5.Text = Config.TelNum5.ToString();
                IP1.Text = Config.IpAddr1.ToString();
                IP2.Text = Config.IpAddr2.ToString();
                IP3.Text = Config.IpAddr3.ToString();
                Port1.Text = Config.IpPort1.ToString();
                Port2.Text = Config.IpPort2.ToString();
                Port3.Text = Config.IpPort3.ToString();  
           
                IP4.Text = Config.IpAddr4.ToString();
                IP5.Text = Config.IpAddr5.ToString();
                Port4.Text = Config.IpPort4.ToString();
                Port5.Text = Config.IpPort5.ToString();
                tbold.Text = Config.Authcode.ToString();
            
        }
        #endregion

        #region 加载默认参数
        private void button5_Click(object sender, EventArgs e)
        {
            bool R;
            COMM_UpdateMsg("加载默认配置参数", 0, " ",1);
            R = CurConfig.ReadConfig(SysConfigFilePath, 0x08);
            COMM_UpdateMsg("", 50, "",1);
         
            
            if (R == true)
            {
                LoadDefaultParams(CurConfig.Config);
               
                COMM_UpdateMsg("", 100, "加载成功!",1);
            }
            else
            {
                MessageBox.Show("默认配置文件加载错误,请检查配置!");
                COMM_UpdateMsg("", 100, "加载失败!",0);
            }
        }
        #endregion



        #region 设为默认值的函数
        private bool SetConfigToDefault()
        {
            float F;
            int T;
           
            if (int.TryParse(tb1.Text.ToString().Trim(), out T))
            {
                CurConfig.Config.DevNumber = T;
            }
            else
            {
                MessageBox.Show("中心用户输入错误!");
                return false;
            }
            if (int.TryParse(tb2.Text.Trim(), out T))
            {
                CurConfig.Config.PasswordCode = T;
            }
            else
            {
                MessageBox.Show("中心密码输入错误!");
                return false;
            }
            if (int.TryParse(tb3.Text.Trim(), out T))
            {
                CurConfig.Config.BaseLevel = T;
            }
            else
            {
                MessageBox.Show("水位基值输入错误!");
                return false;
            }

            if (int.TryParse( Increaselevel.Text.Trim(), out T))
            {
                CurConfig.Config.IncreaseLevel = T;
            }
            else
            {
                MessageBox.Show("水位增量输入错误!");
                return false;
            }


            if (cb5.SelectedItem.ToString().Trim() != null)
            {
                
                if (cb5.SelectedIndex == 0)
                    CurConfig.Config.InterTimeSet = 1;
                if (cb5.SelectedIndex == 1)
                    CurConfig.Config.InterTimeSet = 2;
                if (cb5.SelectedIndex == 2)
                    CurConfig.Config.InterTimeSet = 4;
                if (cb5.SelectedIndex == 3)
                    CurConfig.Config.InterTimeSet = 8;
                if (cb5.SelectedIndex == 4)
                    CurConfig.Config.InterTimeSet = 24;
            }
            else
            {
                MessageBox.Show("整点自报数据输入错误!");
                return false;
            }



            if (tb5.Text.ToString().Trim()!="")
            {
                CurConfig.Config.CompName = tb5.Text.ToString();
            }
            else
            {
                MessageBox.Show("厂商名称输入错误!");
                return false;
            }
            if (tb6.Text.ToString().Trim()!="")
            {
                CurConfig.Config.StationName = tb6.Text.ToString();
            }
            else
            {
                MessageBox.Show("站点名称输入错误!");
                return false;
            }

            if (checkBox1.Checked) CurConfig.Config.WarnEnable = true;
            else CurConfig.Config.WarnEnable = false;

            if (float.TryParse(tb7.Text.Trim(), out F))
            {
                CurConfig.Config.WarningLevel = (int)F; 
            }
            else
            {
                MessageBox.Show("报警水位输入错误!");
                return false;
            }

            if (cb3.SelectedItem.ToString().Trim() != null)
            {
                CurConfig.Config.WarnInterval = int.Parse(cb3.SelectedItem.ToString().Trim());
            }
            else
            {
                MessageBox.Show("报警交隔输入错误!");
                return false;
            }
            if (checkBox1.Checked)
            {
                CurConfig.Config.WarnEnable = true;
            }
            else
            {
                CurConfig.Config.WarnEnable = false;
            }
            if (cb4.SelectedItem.ToString().Trim() != null)
            {
                CurConfig.Config.TelCount = int.Parse(cb4.SelectedItem.ToString().Trim());
            }
            else
            {
                MessageBox.Show("电话个数输入错误!");
                return false;
            }
            if (tbtel1.Text.ToString().Trim()!="")
            { 
                CurConfig.Config.TelNum1 = tbtel1.Text.ToString();
            }
            else
            {
                MessageBox.Show("电话列表号码1输入错误!");
                return false;
            }
            if (tbtel2.Text.ToString().Trim() != "")
            {
                CurConfig.Config.TelNum2 = tbtel2.Text.ToString();
            }
            else
            {
                MessageBox.Show("电话列表号码2输入错误!");
                return false;
            }
            if (tbtel3.Text.ToString().Trim()!="")
            {
                CurConfig.Config.TelNum3 = tbtel3.Text.ToString();
            }
            else
            {
                MessageBox.Show("电话列表号码3输入错误!");
                return false;
            }
            if (tbtel4.Text.ToString().Trim()!="")
            {
                CurConfig.Config.TelNum4 = tbtel4.Text.ToString();
            }
            else
            {
                MessageBox.Show("电话列表号码4输入错误!");
                return false;
            }
            if (tbtel5.Text.ToString().Trim()!="")
            {
                CurConfig.Config.TelNum5 = tbtel5.Text.ToString();
            }
            else
            {
                MessageBox.Show("电话列表号码5输入错误!");
                return false;
            }
            if (IP1.Text.ToString().Trim()!="")
            {
                CurConfig.Config.IpAddr1 = IP1.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP地址列表1输入错误!");
                return false;
            }
            if (IP2.Text.ToString().Trim()!="")
            {
                CurConfig.Config.IpAddr2 = IP2.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP地址列表2输入错误!");
                return false;
            }
            if (IP3.Text.ToString().Trim() != "")
            {
                CurConfig.Config.IpAddr3 = IP3.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP地址列表3输入错误!");
                return false;
            }
            

            if (Port1.Text.ToString().Trim()!="")
            {
                CurConfig.Config.IpPort1 = Port1.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP端口列表1输入错误!");
                return false;
            }
            if (Port2.Text.ToString().Trim() != "")
            {
                CurConfig.Config.IpPort2 = Port2.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP端口列表2输入错误!");
                return false;
            }
            if (Port3.Text.ToString().Trim() != "")
            {
                CurConfig.Config.IpPort3 = Port3.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP端口列表3输入错误!");
                return false;
            }


            if (IP4.Text.ToString().Trim() != "")
            {
                CurConfig.Config.IpAddr4 = IP4.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP地址列表4输入错误!");
                return false;
            }
            if (IP5.Text.ToString().Trim() != "")
            {
                CurConfig.Config.IpAddr5 = IP5.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP地址列表5输入错误!");
                return false;
            }
            if (Port4.Text.ToString().Trim() != "")
            {
                CurConfig.Config.IpPort4 = Port4.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP端口列表4输入错误!");
                return false;
            }
            if (Port5.Text.ToString().Trim() != "")
            {
                CurConfig.Config.IpPort5 = Port5.Text.ToString();
            }
            else
            {
                MessageBox.Show("IP端口列表5输入错误!");
                return false;
            }
            

            return true;
        }

        #endregion

        #region 设为默认值
        private void button6_Click(object sender, EventArgs e)
        {
            COMM_UpdateMsg("保存当前参数为默认参数", 0, " ",1);
            SetConfigToDefault();
            COMM_UpdateMsg("", 30, "",1);
            bool R = CurConfig.SaveConfig(1);       //要保存节点，要保存还几个
            R=CurConfig.SaveConfig(2);       //要保存节点，要保存还几个
            R=CurConfig.SaveConfig(3);       //要保存节点，要保存还几个
            
            ///////////////////////////////////////////?////////////////////////////////////////////////
            if (R == false)
            {
                COMM_UpdateMsg("", 100, "保存失败!",0);
            }
            else
            {
                COMM_UpdateMsg("", 100, "保存成功!",1);
            }
        }
        #endregion




        #region 读取设备参数
        private void button7_Click(object sender, EventArgs e)
        {
            COMM_UpdateMsg("从设备读取配置参数 [0x" + CurConfig.Config.Address.ToString("X2") + "]", 0, " ",1);
            byte[] ConfigDatas = new byte[320];
            int Length =108 * 2 + 5;
            bool R = CommByRS232.ReadPacket(CurConfig.Config.Address, 2004,108, ref ConfigDatas, ref Length);//2004指的寄存器地址
            COMM_UpdateMsg("", 80, "",1);
            if (R == true)
            {
                SysConfig.ConfigParams DevConfig = new SysConfig.ConfigParams();
                //DevConfig.ProductDate = ModbusPacket_GetData32(ConfigDatas, 0);
                DevConfig.DevNumber = ModbusPacket_GetData32(ConfigDatas, 4);                              //偏移量,是指的字节的偏移量
                DevConfig.PasswordCode = ModbusPacket_GetData32(ConfigDatas, 8);
               
                DevConfig.CompName=ModbusPacket_GetString(ConfigDatas, 12, 16);
                DevConfig.StationName = ModbusPacket_GetString(ConfigDatas, 28, 16);

                DevConfig.WarningLevel = ModbusPacket_GetData32(ConfigDatas, 44);
                DevConfig.WarnInterval = ModbusPacket_GetData(ConfigDatas, 48);
                DevConfig.WarnEnable =( ModbusPacket_GetData(ConfigDatas,50)>0)?true:false;

                DevConfig.BaseLevel = ModbusPacket_GetData32(ConfigDatas, 52);
                DevConfig.IncreaseLevel = ModbusPacket_GetData32(ConfigDatas, 56);
                DevConfig.InterTimeSet = ModbusPacket_GetData(ConfigDatas, 60);//指的是字节数ModbusPacket_GetData

                DevConfig.TelCount = ModbusPacket_GetData(ConfigDatas, 62);
                DevConfig.TelNum1 = ModbusPacket_GetString(ConfigDatas, 64, 16);
                DevConfig.TelNum2 = ModbusPacket_GetString(ConfigDatas, 80, 16);
                DevConfig.TelNum3 = ModbusPacket_GetString(ConfigDatas, 96, 16);
                DevConfig.TelNum4 = ModbusPacket_GetString(ConfigDatas, 112, 16);
                DevConfig.TelNum5 = ModbusPacket_GetString(ConfigDatas, 128, 16);

                DevConfig.IpAddr1 = ModbusPacket_GetString(ConfigDatas, 144, 16);
                DevConfig.IpPort1 = ModbusPacket_GetString(ConfigDatas, 160, 8);

                DevConfig.IpAddr2 = ModbusPacket_GetString(ConfigDatas, 168,16);
                DevConfig.IpPort2 = ModbusPacket_GetString(ConfigDatas, 184, 8);

                DevConfig.IpAddr3 = ModbusPacket_GetString(ConfigDatas, 192, 16);
                DevConfig.IpPort3 = ModbusPacket_GetString(ConfigDatas, 208, 8);//220


                Length = 26 * 2 + 5;
                byte[] ConfigDatas1 = new byte[80];
                R = CommByRS232.ReadPacket(CurConfig.Config.Address, 2112, 26, ref ConfigDatas1, ref Length);//指的是寄存器个数
                if (R == true)
                {
                    DevConfig.IpAddr4 = ModbusPacket_GetString(ConfigDatas1, 0, 16);
                    DevConfig.IpPort4 = ModbusPacket_GetString(ConfigDatas1, 16, 8);

                    DevConfig.IpAddr5 = ModbusPacket_GetString(ConfigDatas1, 24, 16);
                    DevConfig.IpPort5 = ModbusPacket_GetString(ConfigDatas1, 40, 8);

                    DevConfig.Authcode = ModbusPacket_GetData32(ConfigDatas1, 48);
                    

                    LoadDefaultParams(DevConfig);
                   
                    COMM_UpdateMsg("", 100, "成功!", 1);
                    tbnew.Text = "";
                    tbnew2.Text = "";

                }
            
             } 
             else
            {
                COMM_UpdateMsg("", 100, "失败!",0);
            }          
           
        }
        #endregion

        #region 添加数据包
        private void ModbusPacket_AddData(ref byte[] Packet, int Offset, int Data)
        {
            Packet[Offset++] = (byte)(Data >> 8);
            Packet[Offset] = (byte)(Data & 0x00FF);
        }
        #endregion



        #region 设置参数
        private void button8_Click(object sender, EventArgs e)
        {
            bool R = false;

            COMM_UpdateMsg("设置参数到设备 [0x" + CurConfig.Config.Address.ToString("X2") + "]", 0, " ",1);

            if (CommByRS232.CommPort.IsOpen == false)
            {
                MessageBox.Show("端口未打开,请先打开端口!");
            }
            else
            {
                R = SetConfigToDefault();
                COMM_UpdateMsg("", 20, "",1);

                if (R == true)
                {
                    byte[] ConfigDatas = new byte[320];

                    ModbusPacket_AddData32(ref ConfigDatas, 0, CurConfig.Config.DevNumber);
                    ModbusPacket_AddData32(ref ConfigDatas, 4, CurConfig.Config.PasswordCode);
                    ModbusPacket_GetByte(ref ConfigDatas,CurConfig.Config.CompName, 8, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.StationName, 24, 16);
                 
                    ModbusPacket_AddData32(ref ConfigDatas, 40, (int)CurConfig.Config.WarningLevel);
                    ModbusPacket_AddData(ref ConfigDatas, 44, CurConfig.Config.WarnInterval);
                    ModbusPacket_AddData(ref ConfigDatas,46, CurConfig.Config.WarnEnable==true?1:0);

                    ModbusPacket_AddData32(ref ConfigDatas, 48, CurConfig.Config.BaseLevel);//预留的 for reserved
                    ModbusPacket_AddData32(ref ConfigDatas, 52, CurConfig.Config.IncreaseLevel);
                    ModbusPacket_AddData(ref ConfigDatas, 56, CurConfig.Config.InterTimeSet);
                   
                    ModbusPacket_AddData(ref ConfigDatas, 58, CurConfig.Config.TelCount);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.TelNum1, 60, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.TelNum2, 76, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.TelNum3, 92, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.TelNum4, 108, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.TelNum5, 124, 16);

                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.IpAddr1, 140, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.IpPort1, 156, 8);

                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.IpAddr2, 164, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.IpPort2, 180, 8);

                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.IpAddr3, 188, 16);
                    ModbusPacket_GetByte(ref ConfigDatas, CurConfig.Config.IpPort3, 204, 8);

                    COMM_UpdateMsg("", 40, "", 1);
                  
                    R = CommByRS232.WritePacket(CurConfig.Config.Address, 2006, 106, ConfigDatas, 212);

                    byte[] ConfigDatas1 = new byte[80];
                                      
                    if (R == true)
                    {
                        ModbusPacket_GetByte(ref ConfigDatas1, CurConfig.Config.IpAddr4, 0, 16);
                        ModbusPacket_GetByte(ref ConfigDatas1, CurConfig.Config.IpPort4, 16, 8);
                       

                        ModbusPacket_GetByte(ref ConfigDatas1, CurConfig.Config.IpAddr5, 24, 16);
                        ModbusPacket_GetByte(ref ConfigDatas1, CurConfig.Config.IpPort5, 40, 8);
                      
                        //ModbusPacket_AddData32(ref ConfigDatas1, 48, CurConfig.Config.Authcode);
                       
                        COMM_UpdateMsg("", 80, "", 1);

                        R = CommByRS232.WritePacket(CurConfig.Config.Address, 2112, 24, ConfigDatas1, 48);

                    }
                                  
                }
            }
            if (R == true)
            {
                COMM_UpdateMsg("", 100, "成功!",1);
            }
            else
            {
                COMM_UpdateMsg("", 100, "失败!",0);
            }
        }
        #endregion

        #region 修改密码
        private void button10_Click(object sender, EventArgs e)
        {
            COMM_UpdateMsg("", 0, " ",0);
            if (CommByRS232.CommPort.IsOpen == false)
            {
                MessageBox.Show("端口未打开,请先打开端口!");
                return;
            }

            byte[] ConfigDatas = new byte[50];
            bool R = false;
            
            if (tbold.Text.ToString().Trim() == "")
            {
                COMM_UpdateMsg("", 0, "密码不能为空!", 0);
                MessageBox.Show("请输入密码 ", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            if (tbnew.Text.ToString().Trim() == "" || tbnew2.Text.ToString().Trim() == "")
            {
                COMM_UpdateMsg("", 0, "新密码不能为空!", 0);
                MessageBox.Show("新密码不能为空! ", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
   
           
            if (tbnew.Text.ToString().Trim() != tbnew2.Text.ToString().Trim())
            {
                COMM_UpdateMsg("", 0, "两次输入的密码不一致!", 0);
                MessageBox.Show("两次输入的密码不一致! ", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }

            //  正则表达式只能输入n位的数字："^\d{n}$"。
            try
            {
                int.Parse(tbnew.Text.ToString());

            }
            catch
            {
                COMM_UpdateMsg("", 0, "密码长度不为6或必须为数字!", 0);
                MessageBox.Show("密码长度不为6或必须为数字! ", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
                return;
            }
            //if (tbnew.Text != @"^\d{6}$" )
            //{

            //    COMM_UpdateMsg("", 0, "密码长度不为6或必须为数字!", 0);
            //    MessageBox.Show("密码长度不为6或必须为数字! ", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
            //    return;
            //}
            /*
             * static   private   Regex   r   =   new   Regex( "^[0-9]{1,}$ ");       //这个可以写成静态的，就不用老是构造 
            if(!r.IsMatch(textBox1.Text)) 
           { 
             Messagebox.Show( "请输入数字 "); 
           }
             */

            CurConfig.Config.Authcode =int.Parse( tbnew2.Text.ToString().Trim());
            ModbusPacket_AddData32(ref ConfigDatas, 0, CurConfig.Config.Authcode);
            R = CommByRS232.WritePacket(CurConfig.Config.Address, 2136, 2, ConfigDatas, 4);

            if (R == true)
            {
                COMM_UpdateMsg("", 100, "成功!", 1);
                MessageBox.Show("修改密码成功! ", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            else
            {
                COMM_UpdateMsg("", 100, "失败!", 0);
            }

        }     
        #endregion

        #region 密码初始化
        private void button11_Click(object sender, EventArgs e)
        {
            byte[] ConfigDatas = new byte[50];
            bool R = false;
            COMM_UpdateMsg("", 0, " ", 0);
        
      
            CurConfig.Config.Authcode = 123456;
            ModbusPacket_AddData32(ref ConfigDatas,0, CurConfig.Config.Authcode);

            R = CommByRS232.WritePacket(CurConfig.Config.Address, 2136, 2, ConfigDatas,4);

            if (R == true)
            {
                COMM_UpdateMsg("", 100, "修改密码成功!", 1);
                MessageBox.Show("初始密码为123456! ", "消息", MessageBoxButtons.OK, MessageBoxIcon.None);
                tbold.Text = CurConfig.Config.Authcode.ToString();

            }
            else
            {
                COMM_UpdateMsg("", 100, "失败!", 0);
            }
            

        }
        #endregion

        #region tabcontrol进行变化时
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 || tabControl1.SelectedIndex == 2)
            {
                //button13.Enabled = true;
                //button14.Enabled = true;
            }
            else if (tabControl1.SelectedIndex == 0)
            {
                //button13.Enabled = false;
                //button14.Enabled = false;
            }

        }
        #endregion      

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabel7.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");          
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurConfig.Config.Address = int.Parse(comboBox4.Text.Trim());

        }



       

       
      

       

    }
}
