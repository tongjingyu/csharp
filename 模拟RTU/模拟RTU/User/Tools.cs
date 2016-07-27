using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
namespace 模拟RTU
{
    class Tools
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Union
        {
            [FieldOffset(0)]
            public UInt32 U32;
            [FieldOffset(0)]
            public Int32 I32;
            [FieldOffset(0)]
            public float F;
            [FieldOffset(0)]
            public byte B0;
            [FieldOffset(1)]
            public byte B1;
            [FieldOffset(2)]
            public byte B2;
            [FieldOffset(3)]
            public byte B3;
        }
        public static int FInArray(ref Byte[] Datas, float F, int Offset, int Mode)
        {
            Union temp;
            temp.B0 = 0;
            temp.B1 = 0;
            temp.B2 = 0;
            temp.B3 = 0;
            temp.F =F;
            if (Mode == 0)
            {
                Datas[Offset] = temp.B0;
                Datas[Offset + 1] = temp.B1;
                Datas[Offset + 2] = temp.B2;
                Datas[Offset + 3] = temp.B3;
            }
            if (Mode == 1)
            {
                Datas[Offset] = temp.B3;
                Datas[Offset+1] = temp.B2;
                Datas[Offset + 2] = temp.B1;
                Datas[Offset + 3] = temp.B0;
            }
            return Offset+4;
        }
        public static int D8InArray(ref Byte[] Datas, int D, int Offset, int Mode)
        {
            int S = 0xff;
            if (Mode == 0)
            {
                Datas[Offset] = (byte)(D & S);
                
            }
            if (Mode == 1)
            {
                D>>=8;
                Datas[Offset] = (byte)(D & S);
            }
            return Offset + 1;
        }
        public static int D16InArray(ref Byte[] Datas,int D,int Offset,int Mode)
        {
            int S = 0xff;
            if (Mode == 0)
            {    //小端格式
                Datas[Offset] =(byte)(D &S); D >>= 8;
                Datas[Offset+1]=(byte)(D&S);
            }
            else if (Mode == 1)
            {   //大端格式
                Datas[Offset+1] =(byte)(D &S); D >>= 8;
                Datas[Offset] =(byte)(D&S);
            }
            return Offset + 2;
        }
        public static int D32InArray(ref Byte[] Datas, int D, int Offset, int Mode)
        {
            int S = 0xff;
            if (Mode == 0)
            {
                Datas[Offset] =(byte)(D &S); D>>=8;
                Datas[Offset + 1] = (byte)(D & S); D >>= 8;
                Datas[Offset + 2] = (byte)(D & S); D >>= 8;
                Datas[Offset + 3] = (byte)(D & S);
            }
            if(Mode==1)
            {
                Datas[Offset + 3] = (byte)(D & S); D >>= 8;
                Datas[Offset+2]=(byte)(D&S);D>>=8;
                Datas[Offset+2]=(byte)(D&S);D>>=8;
                Datas[Offset+1]=(byte)(D&S);D>>=8;
                Datas[Offset]=(byte)(D&S);
            }
            return Offset + 4;
        }
        public static int StrInArray(ref byte[] Datas, string S, int Offset, int Mode)
        {
            byte[] Buf=new byte[100];
            Buf= Encoding.GetEncoding("gb2312").GetBytes(S);
            int Length=Buf.Length;
            if (Mode == 0)
            {
                for (int i = 0; i < Length; i++) Datas[Offset++]=Buf[i];
            }
            if (Mode == 1)
            {
                for (int i = Length; i > 0; i++) Datas[Offset++] = Buf[i];
            }
            return Offset;
        }
        public static int DAT_GetD32FromArray(byte[] Datas, int Offset, int Mode)
        {
            int T = 0;
            if (Mode == 0)
            {    //小端格式
                T = Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset];
            }
            else if (Mode == 1)
            {   //大端格式
                T = Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 3];
            }
            else if (Mode == 2)
            {
                T = Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset + 2];
            }
            else if (Mode == 3)
            {
                T = Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 1];
            }
            return T;
        }
        public static float ByteToFloat(byte[] Buffer, int Offset, int Mode)
        {
            Union temp;
            temp.F = 0;
            if (Mode == 0)
            {
                temp.B0 = Buffer[Offset];
                temp.B1 = Buffer[Offset + 1];
                temp.B2 = Buffer[Offset + 2];
                temp.B3 = Buffer[Offset + 3];
            }
            if (Mode == 1)
            {
                temp.B3 = Buffer[Offset];
                temp.B2 = Buffer[Offset + 1];
                temp.B1 = Buffer[Offset + 2];
                temp.B0 = Buffer[Offset + 3];
            }
            if (Mode == 2)
            {
                temp.B3 = Buffer[Offset + 2];
                temp.B2 = Buffer[Offset + 3];
                temp.B1 = Buffer[Offset + 0];
                temp.B0 = Buffer[Offset + 1];
            }
            if (Mode == 3)
            {
                temp.B3 = Buffer[Offset + 1];
                temp.B2 = Buffer[Offset + 0];
                temp.B1 = Buffer[Offset + 3];
                temp.B0 = Buffer[Offset + 2];
            }
            float F = temp.F;
            return F;
        }
        public static int ToUnsigned(int Nfm, int Nf)
        {
            int Temp = 0;
            if (Nfm > 0)
            {
                Temp = 0x07 & (~Nf) + 1;
            }
            else Temp = Nf;
            return Temp;
        }
        public static string DAT_GetNum(byte[] Buffer, int Offset, int Mode, int Type)
        {
            int Nf = Type & 0x07;
            int Nfm = Type & 0x08;
            Type = Type >> 4;
            string Data = "NULL";
            uint TempBF = 0;
            float TempF = 0;
            // Console.WriteLine("{0},{1},{2}",Nf, Nfm, Type);
            switch (Type)
            {
                case 0://浮点型
                    Data = (ByteToFloat(Buffer, Offset, Mode)).ToString();
                    break;
                case 1://有符号长整形
                    Int32 TempI32 = DAT_GetD32FromArray(Buffer, Offset, Mode);
                    TempBF = (uint)Math.Pow(10, ToUnsigned(Nfm, Nf));
                    TempF = TempI32;
                    if (Nfm == 8) TempF /= TempBF;
                    else TempF *= TempBF;
                    Data = (TempF).ToString();
                    break;
                case 9://无符号长整形
                    UInt32 TempU32 = (UInt32)DAT_GetD32FromArray(Buffer, Offset, Mode);
                    TempBF = (uint)Math.Pow(10, ToUnsigned(Nfm, Nf));
                    TempF = (float)TempU32;
                    if (Nfm == 8) TempF /= TempBF;
                    else TempF *= TempBF;
                    Data = (TempF).ToString();
                    break;
                default: break;
            }
            UInt32 Temp = (UInt32)DAT_GetD32FromArray(Buffer, Offset, Mode);
            if (Temp == 0x80000000) Data = "NULL";
            return Data;
        }
        public static bool IfNotFileCreat(string Path)
        {
            if (!Directory.Exists(Path))
            {
                try
                {
                    Directory.CreateDirectory(Path);
                    return true;
                }
                catch { return false; }
            }
            else return true;
        }
        public static int DateTimeInArray(ref byte[] Datas, DateTime DT, int Offset, int Mode)
        {
            if (Mode == 0)
            {
                Offset  =D16InArray(ref Datas, DT.Year,Offset,0);
                Offset = D8InArray(ref Datas, DT.Month, Offset, 0);
                Offset = D8InArray(ref Datas, DT.Day, Offset, 0);
                Offset = D8InArray(ref Datas, DT.Hour, Offset, 0);
                Offset = D8InArray(ref Datas, DT.Minute, Offset, 0);
            }
            if (Mode == 1)
            {
                Offset = D8InArray(ref Datas, DT.Minute, Offset, 0);
                Offset = D8InArray(ref Datas, DT.Hour, Offset, 0);
                Offset = D8InArray(ref Datas, DT.Day, Offset, 0);
                Offset = D8InArray(ref Datas, DT.Month, Offset, 0);
                Offset = D16InArray(ref Datas, DT.Year, Offset, 1);
            }
            return Offset;
        }
        public static string SendRead(string Msg)
        {
            if (SysFlag.ServiceLink == false) return SysFlag.ServiceLink.ToString();

             try
             {
                 string Temp = "begin " + Msg;
                 Msg = Temp;
                 NetworkStream SendStream = Configs.Client_New.GetStream();
                 byte[] SendBuffer = new byte[1000];
                 byte[] ReadBuffer = new byte[1000];
                 SendBuffer = Encoding.GetEncoding("gb2312").GetBytes(Msg);
                 Configs.Client_New.SendTimeout = 100;
                 SendStream.Write(SendBuffer, 0, SendBuffer.Length);
                 SendStream.ReadTimeout = 100;
                 int RxLength;
                 try
                 {
                     RxLength = SendStream.Read(ReadBuffer, 0, 1000);
                 }
                 catch { SysFlag.ServiceLink = false; return "False"; }
                 Msg = Encoding.GetEncoding("gb2312").GetString(ReadBuffer, 0, RxLength);
             }
             catch { SysFlag.ServiceLink = false; return "False"; }
             return Msg;
        }
        public static bool LinkService()
        {
            IPEndPoint IpPoint;
            if (Configs.ServiceHost) IpPoint = Tools.Get_HostIpPoint(Configs.UserFacePoint);
            else IpPoint = new IPEndPoint(IPAddress.Parse(Configs.ServiceAddr), Configs.UserFacePoint);
            if (!SysFlag.ServiceLink)
            {
                    try
                    {
                        Configs.Client_New = new TcpClient();
                        Configs.Client_New.SendTimeout = 100;
                        Configs.Client_New.Connect(IpPoint);
                        SysFlag.ServiceLink = true;
                        return true;
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show(E.Message, "连接服务器失败！");
                        SysFlag.ServiceLink = false;
                        return false;
                    }
            }else return true;
        }
        public static bool SaveAll(ref string Lost)
        {
            SaveFileDialog sf = new SaveFileDialog(); //对话框
            sf.Filter = @"文件(*."+Lost+")|*."+Lost+"|所有文件(*.*)|*.*"; //定义保存的文件的类型
            StringBuilder strHtml = new StringBuilder();
            if (sf.ShowDialog() == DialogResult.OK) //如果确定保存
            {
                Lost = sf.FileName;
                return true;
            }
            else return false;
        }
        public static bool Login(string Name, string PassWord)
        {
            if (SysFlag.ServiceLink)
            {
                string Msg = "Login " + Name + " " + PassWord;
                try
                {
                    NetworkStream SendStream =Configs.Client_New.GetStream();
                    byte[] SendBuffer = new byte[1000];
                    byte[] ReadBuffer = new byte[1000];
                    SendBuffer = Encoding.GetEncoding("gb2312").GetBytes(Msg);
                    Configs.Client_New.SendTimeout = 100;
                    SendStream.Write(SendBuffer, 0, SendBuffer.Length);
                    SendStream.ReadTimeout = 100;
                    int RxLength;
                    RxLength = SendStream.Read(ReadBuffer, 0, 1000);
                    Msg = Encoding.GetEncoding("gb2312").GetString(ReadBuffer, 0, RxLength);
                    if (Msg == "LoginOk") return true;
                    else
                    {
                        MessageBox.Show("登录名或登录密码错误!", "登录失败");
                        return false;
                    }
                }
                catch(Exception E)
                {
                    MessageBox.Show(E.Message,"连接服务器断开!");
                    SysFlag.ServiceLink = false;
                    return false;
                }
            }
            else
            {
                MessageBox.Show("没有可用服务器连接!");
                return false;
            }
            
        }
        public static IPEndPoint Get_HostIpPoint(int Point)
        {
            IPAddress IPAddr = IPAddress.Parse("127.0.0.1");
            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIPAddresses)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    IPAddr = ip;
                }
            }
            IPEndPoint Ipep = new IPEndPoint(IPAddr, Point);
            return Ipep;
        }
        public static bool ToHtml(DataGridView GridView1, DataGridView GridView, string Title)
        {
            string PathName = "";
            SaveFileDialog sf = new SaveFileDialog(); //对话框
            sf.Filter = @"html文件(*.html)|*.html|所有文件(*.*)|*.*"; //定义保存的文件的类型
            string Temp = "";
            StringBuilder strHtml = new StringBuilder();
            if (sf.ShowDialog() == DialogResult.OK) //如果确定保存
            {
                PathName = sf.FileName;
            }
            else return false;
            strHtml.Append("<html><head><center><ol><font size='6'>设备信息</font></li><table width='300' border='1' bordercolor='blue' cellspacing='0'>\r\n");
            try
            {
                for (int i = 0; i < GridView1.ColumnCount; i++)
                {
                    strHtml.Append("<th><NOBR>" + GridView1.Columns[i].HeaderText + "</NOBR></th>");
                }
                strHtml.Append("\r\n");
                for (int i = 0; i < GridView1.Rows.Count - 1; i++) //循环写入dataGridView中的内容
                {
                    strHtml.Append("<tr> ");
                    for (int j = 0; j < GridView1.Columns.Count; j++)
                    {
                        strHtml.Append("<td><NOBR>");
                        strHtml.Append(GridView1.Rows[i].Cells[j].Value.ToString().Trim());
                        if (GridView1.Rows[i].Cells[j].Value == null) Temp += "    ";
                        strHtml.Append("</NOBR></td>");
                    }
                    strHtml.Append("</tr>\r\n");
                }   
            }     catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
                strHtml.Append("</ol><br></table><br><hr><br></center><br>");
                strHtml.Append("<center><ol><font size='6'>" + Title + "</font></li><table width='300' border='1' bordercolor='blue' cellspacing='0'>\r\n");
                try
                {
                    for (int i = 0; i < GridView.ColumnCount; i++)
                    {
                        strHtml.Append("<th><NOBR>" + GridView.Columns[i].HeaderText + "</NOBR></th>");
                    }
                    strHtml.Append("\r\n");
                    for (int i = 0; i < GridView.Rows.Count - 1; i++) //循环写入dataGridView中的内容
                    {
                        strHtml.Append("<tr> ");
                        for (int j = 0; j < GridView.Columns.Count; j++)
                        {
                            strHtml.Append("<td><NOBR><center>");
                            strHtml.Append(GridView.Rows[i].Cells[j].Value.ToString().Trim());
                            if (GridView.Rows[i].Cells[j].Value.ToString().Trim() == "") strHtml.Append("缺省");
                            strHtml.Append("</NOBR></center></td>");
                        }
                        strHtml.Append("</tr>\r\n");
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message, "导出失败!");
                    return false;
                }
                strHtml.Append("</ol><br></table><br><hr><br><font color='red'>湖北亿立能科技有限公司 copyright@2013</center>");
                try
                {
                    Encoding encod = Encoding.Default;
                    byte[] buff = new byte[encod.GetByteCount(strHtml.ToString())];
                    encod.GetBytes(strHtml.ToString(), 0, strHtml.Length, buff, 0);
                    FileStream fs = new FileStream(PathName, FileMode.Create);
                    fs.Write(buff, 0, buff.Length);
                    fs.Close();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message, "导出失败!");
                    return false;
                }
                MessageBox.Show("导出成功!");
                return true;
        }
    }
}
