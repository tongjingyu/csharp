using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.IO;
namespace RTUService
{
    class ManageOrder
    {
        public string Oder1;
        public string Oder2;
        public string Oder3;
        public string Oder4;
        public string Oder5;
        public string Oder6;
        public string Oder7;
        public string Oder8;
        public string Oder9;
        public ManageOrder(string Msg)
        {
            string[] Arry = Msg.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int i=0;
            int ArryLength=Arry.Length;
            if (ArryLength > i) this.Oder1 = Arry[i++].ToLower();
            if (ArryLength > i) this.Oder2 = Arry[i++].ToLower();
            if (ArryLength > i) this.Oder3 = Arry[i++].ToLower();
            if (ArryLength > i) this.Oder4 = Arry[i++];
            if (ArryLength > i) this.Oder5 = Arry[i++];
            if (ArryLength > i) this.Oder6 = Arry[i++];
            if (ArryLength > i) this.Oder7 = Arry[i++];
            if (ArryLength > i) this.Oder8 = Arry[i++];
            if (ArryLength > i) this.Oder9 = Arry[i++];
        }

    }

    class UserFaceThread
    {
        public static bool CreateUser()
        {
            int Count=0;
            while (SysFlag.ServiceStart && Count++ < 1000)
            {
                try
                {
                    Configs.UserSock.Bind(Tools.Get_IpPoint(Configs.UserFacePoint));//绑定
                    break;
                }
                catch { System.Threading.Thread.Sleep(100);}
            }
                Configs.UserSock.Listen(1000);//监听
                Configs.UserSock.ReceiveTimeout = 1000;
                Configs.UserSock.SendTimeout = 1000;
            if(Count>990)
            {
                CreateInfor.WriteLogs("【申请用户交互端口失败】，请释放或更改端口!");                
                SysFlag.ServiceStart = false;//服务启动失败
                return false;
            }
            CreateInfor.WriteLogs("第【"+Count.ToString()+"】次申请用户交互端口才成功!");
            Thread UserFaceThread= new Thread(new ThreadStart(FaceThread));
            UserFaceThread.Start();
            CreateInfor.WriteLogs("多用户接口创建成功!使用通道【" + Tools.Get_IpPoint(Configs.UserFacePoint).ToString() + "】");
            return true;
        }
        private static void FaceThread()
        {
                UserFace.MainThread();
        }
    }
    class UserFace
    {
         
        public Socket service;
        public UserFace(Socket clientsocket)
        {
            this.service = clientsocket;
        }
        public static void MainThread()
        {
            while (SysFlag.ServiceStart)
            {
                try
                {
                    Socket client = Configs.UserSock.Accept();//当有可用的客户端连接尝试时执行，并返回一个新的socket,用于与客户端之间的通信
                    UserFace newclient = new UserFace(client);
                    Thread newthread = new Thread(new ThreadStart(newclient.ClientService));
                    newthread.Start();
                    if (!SysFlag.ServiceStart) { client.Close(); return; }
                }
                catch (Exception E)
                {
                    CreateInfor.WriteLogs(E.Message+"导致【错误处于客户端侦听循环】");
                }
            }
            Configs.UserSock.Close();
            CreateInfor.WriteLogs("用户访问通道关闭!");
        }
        public bool ReadLogin(NetworkStream NewStream)
        {
            byte[] RxBuffer = new byte[Configs.DataByteSize];
            while (true)
            {
                try
                {
                    int RxLength = NewStream.Read(RxBuffer, 0, Configs.DataByteSize);
                    string Msg = Encoding.GetEncoding("gb2312").GetString(RxBuffer, 0, RxLength);
                    ManageOrder MO = new ManageOrder(Msg);
                    if (MO.Oder1 == "login")
                    {
                        if (XML.ReadLogin(MO.Oder2, MO.Oder3))
                        {
                            RxBuffer = Encoding.GetEncoding("gb2312").GetBytes("True");
                            NewStream.Write(RxBuffer, 0, RxBuffer.Length);
                            return true;
                        }
                    }
                    RxBuffer = Encoding.GetEncoding("gb2312").GetBytes("False");
                    NewStream.Write(RxBuffer, 0, RxBuffer.Length);
                }
                catch { return false; }
            }
        }
        public void ClientService()
        {
            byte[] RxBuffer = new byte[Configs.DataByteSize];
            byte[] sendbytes = new byte[Configs.DataByteSize];
            int RxLength;
            bool LoginOk = false;
            if (service != null)
            {
                 SysFlag.UserCount++;
            }   

            try
            {
                NetworkStream NewStream = new NetworkStream(service);
                NewStream.ReadTimeout = Configs.UserTimeOut;
              //  if (!ReadLogin(NewStream)) return;
                while (SysFlag.ServiceStart)
                {
                    try
                    {
                        RxLength = NewStream.Read(RxBuffer, 0, Configs.DataByteSize);
                        if (RxLength > 0)
                        {
                            string UFA = UserFaceAnswered(RxBuffer, RxLength, ref LoginOk);
                            if (UFA != "")
                            {
                                sendbytes = Encoding.GetEncoding("gb2312").GetBytes(UFA);
                                NewStream.Write(sendbytes, 0, sendbytes.Length);
                            }
                        }
                        else break;
                    }
                    catch (TimeoutException E) { }
                    catch { ; break; }
                }
                service.Close();
            }
            catch { CreateInfor.WriteLogs( "程序关闭延迟导致【用户接口读取连接用户数据失败】"); }
            SysFlag.UserCount--;
        }
        public string  UserFaceAnswered(byte[] RxBuffer, int RxLength,ref bool LoginOk)
        {
            string Msg = Encoding.GetEncoding("gb2312").GetString(RxBuffer, 0, RxLength);
            ManageOrder MO = new ManageOrder(Msg);
            if (MO.Oder1 == "login")if (XML.ReadLogin(MO.Oder2, MO.Oder3))
            {
                CreateInfor.WriteLogs("用户【"+MO.Oder2+"】登录!");
                LoginOk = true;
                return "LoginOk";
            }
            if (!LoginOk) return "LoginError";
            if(MO.Oder1=="begin")
            {
                switch (MO.Oder2)
                {   
                    case "get": switch (MO.Oder3)
                        {
                        case "end":break;
                        case "clientcount": return (SysFlag.ClientCount.ToString());
                        case "usercount": return (SysFlag.UserCount.ToString());
                        case "sqlconnectok": return (SysFlag.SqlConnectOk.ToString());
                        case "sqlpassword": return (Configs.SqlPassWord);
                        case "sqlname":return (Configs.SqlName);
                        case "sqldatabase": return (Configs.SqlDataBase);
                        case "sqldatasource":return(Configs.SqlDataSource);
                        case "sqldensitysheet": return (Configs.SqlDensitySheet);
                        case "sqlrealtimesheet":return (Configs.SqlRealTimeSheet);
                        case "sqlstationinfor": return (Configs.SqlStationInforSheet);
                        case "configspath": return Configs.Path;
                        case "densityqueuesize": return (Configs.DensityPacketQueue.Count.ToString());
                        case "realtimequeuesize": return (Configs.RealTimePacketQueue.Count.ToString());
                        case "listenok":return (SysFlag.ListenOk.ToString());
                        case "runpath":return (Directory.GetCurrentDirectory());
                        case "realtimepacketcount":return (SysFlag.RealTimePacketCount.ToString());
                        case "densitypacketcount":return (SysFlag.DensityPacketCount.ToString());
                        }break;
                    case "set": switch (MO.Oder3)
                        {
                            case "sqlname": return (XML.SetConfigS(ref Configs.SqlName, MO.Oder4).ToString());
                            case "sqlpassword": return (XML.SetConfigS(ref Configs.SqlPassWord, MO.Oder4).ToString());
                            case "sqldatasource": return (XML.SetConfigS(ref Configs.SqlDataSource, MO.Oder4).ToString());
                            case "sqldatabase": return (XML.SetConfigS(ref Configs.SqlDataBase, MO.Oder4).ToString());
                            case "sqldensitysheet": return (XML.SetConfigS(ref Configs.SqlDensitySheet, MO.Oder4).ToString());
                            case "sqlrealtimesheet": return (XML.SetConfigS(ref Configs.SqlRealTimeSheet, MO.Oder4).ToString());
                            case "servicepoint": return (XML.SetConfigD(ref Configs.ServicePoint, MO.Oder4).ToString());
                            case "sqlstationinforsheet": return (XML.SetConfigS(ref Configs.SqlStationInforSheet, MO.Oder4).ToString());
                            case "userrootpassword":return (XML.SetConfigS(ref Configs.UserRootPassWord,MO.Oder4).ToString());
                        } break;
                    case "open":switch(MO.Oder3)
                        {
                        case "end": break;
                        }break;
                    case "restart": switch (MO.Oder3)
                        {
                        case "end":break;
                        } break;
                    default: break;
                }
            }
            return "False";
        }
    }
}
