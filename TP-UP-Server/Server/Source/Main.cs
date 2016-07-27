using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Management;
namespace Server
{
    class Main
    {
        public static TcpListener TcpLiserner;


        public void MainThread()
        {
            Value.Path="C:\\TPServer";
            Value.CFG = new Configs(Value.Path);
            Value.WriteLog = new Log(Value.Path, LogType.LT_All);
            Value.CFG.Load();
            Value.fileIO = new FileIO(Value.Path);
            CreateServiceThread();
        }
        public static bool CreateServiceThread()
        {
            int Count = 0;
            while (TagetValue.Run && Count++ < 1000)
            {
                try
                {
                    TcpLiserner = new TcpListener(IPAddress.Any,Value.ServerPort);
                    TcpLiserner.Start();
                    break;
                }
                catch { System.Threading.Thread.Sleep(100); }
            }

            Value.WriteLog.WriteLine("监听ALL["+Value.ServerPort+"]", LogType.LT_Warning);
            Thread ServiceThread = new Thread(new ThreadStart(MainListenThread));
            ServiceThread.Start();
            return true;
        }
        public static void MainListenThread()
        {
            while (TagetValue.Run)
            {
                try
                {
                    Socket client = TcpLiserner.AcceptSocket();
                    ClientThread newclient = new ClientThread(client);
                     IPEndPoint clientip = (IPEndPoint)client.RemoteEndPoint;
                    Value.WriteLog.WriteLine("new Client:" + clientip.Address+":"+clientip.Port, LogType.LT_Warning);
                    Thread newthread = new Thread(new ThreadStart(newclient.ClientService));
                    newthread.Start();
                    if (!TagetValue.Run) { client.Close(); return; }
                }
                catch (Exception E)
                {
                    Value.WriteLog.WriteLine("MainListenThread:" + E.Message, LogType.LT_Warning);
                }
            }
            
        }
    }

    class ClientThread
    {
        public static int connections = 0;
        public Socket service;
        public ClientThread(Socket clientsocket)
        {
            this.service = clientsocket;
        }
        private Encoding charEncoder = Encoding.GetEncoding("GB2312"); // To encode string
        private string GetString(string Key)
        {
            string Msg = "";
            try
            {
                string[] ary;
                ary = Tools.GetHaseKey(Key, Value.hs);
                if (ary.Length == 0) return "";
                for (int i = 0; i < ary.Length; i++)
                {
                    string[] array = ary[i].Split(':');
                    if (array.Length > 1)
                    {
                       
                        Msg += Html.AddTextBoxLable("TextBox" + i, array[1], array[0], 30);
                    }
                }
            }
            catch (Exception E) { Value.WriteLog.WriteLine("GetString:" + E.Message, LogType.LT_Warning); }
            return Msg;
        }
        private void sendButtonResponse(Socket clientSocket,string key)
        {
            string keystr = GetString(key);
            bool online = false;
            if (keystr!="") online = true;
            string msg = Html.AddHead(online) +
             keystr +
             Html.AddrTwoButton("检查", "提交", 30) +
             Html.AddEnd();
            byte[] bHeader = charEncoder.GetBytes(msg);
            Value.WriteLog.WriteLine("发送:" + msg, LogType.LT_Warning);
            clientSocket.Send(bHeader);
        }
        public void ClientService()
        {
            byte[] RxBuffer = new byte[1500];
            byte[] TxBuffer = new byte[1500];
            bool DevTrue = false;
            byte[] BinFile=new byte[0];
            HashKey HK = new HashKey();
            string[] TextFile = new string[0];
            byte[] BinCFG = new byte[0];
            int fileBlackSize=0;
            service.ReceiveTimeout = 10000;
            while (TagetValue.Run)
            {
            try
            {
                       
                int RxLength = service.Receive(RxBuffer);
                string Msg=System.Text.Encoding.GetEncoding("GB2312").GetString(RxBuffer);
                if (Msg.IndexOf("GET /")==0)
                {
                    Value.WriteLog.WriteLine("Msg:" + Msg, LogType.LT_Warning);
                    sendButtonResponse(service, Html.GetHttpValue(Msg));
                    Value.WriteLog.WriteLine("key:" + Html.GetHttpValue(Msg), LogType.LT_Warning);
                    DevTrue = false;
                    service.Close();
                    return;
                }
                if (Msg.IndexOf("POST /")==0)
                {
                    Value.WriteLog.WriteLine("Msg:" + Msg, LogType.LT_Warning);
                    if (Msg.IndexOf("ButtonSubmit") < 0)
                    {
                        int receivedBCount = service.Receive(RxBuffer);
                        Msg +=charEncoder.GetString(RxBuffer, 0, receivedBCount);
                        Value.WriteLog.WriteLine("接收2:" + Msg, LogType.LT_Warning);
                        Value.WriteLog.WriteLine("key:" + Html.GetHttpValue(Msg), LogType.LT_Warning);
                    }
                    DevTrue = false;
                    Tools.ReChange(Msg);
                    sendButtonResponse(service, Html.GetHttpValue(Msg));
                    service.Close();
                    return;
                }
                if (service.Poll(10, SelectMode.SelectRead))
                {
                    Value.WriteLog.WriteLine("客户断开", LogType.LT_Warning);
                    Value.DevOnlineCount--;
                    service.Close();
                    if(DevTrue)Tools.RemoveHK(HK.OnlyTime);
                    return;
                }
                if(RxLength>1)
                    if (ZBUS.ZBUS_CheckCrc(RxBuffer))
                    {
                        DevTrue = true;
                        string str = Tools.HexToString(RxBuffer, RxLength);
                        int ID = ((int)RxBuffer[0]) * 0xff + (int)RxBuffer[1];
                        Value.WriteLog.WriteLine("Revice:" + str, LogType.LT_Warning);
                        switch (RxBuffer[2])
                        {
                            case (byte)UpGradeCmd.UGC_SetBinFile://设置焦点文件设置块大小
                                    fileBlackSize = RxBuffer[5];
                                    fileBlackSize <<= 8;
                                    fileBlackSize += RxBuffer[6];
                                    Value.WriteLog.WriteLine("Revice:" + fileBlackSize, LogType.LT_Warning);
                                    RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UGC_SetBinFile|0x80, null, 0);
                                    Value.WriteLog.WriteLine("Send:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                    service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                                    Value.WriteLog.WriteLine("LoadBinFile:" +ID , LogType.LT_Warning);
                                    BinFile = Value.fileIO.ReadBin(ID);break;
                            case (byte)UpGradeCmd.UGC_GetBinFileInfor://获取文件信息
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UGC_GetBinFileInfor | 0x80, Tools.ByteFromU32((UInt32)BinFile.Length, 0).Concat(Tools.GetCrc16byte(BinFile, 0, BinFile.Length)).ToArray(), 6);
                                    Value.WriteLog.WriteLine("Send:" + Tools.HexToString                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       (TxBuffer, RxLength), LogType.LT_Warning);
                                    service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());break;
                            case (byte)UpGradeCmd.UGC_ReadBinFile:
                                byte[] Data = new byte[0];
                                if ((fileBlackSize * ID + fileBlackSize) <= BinFile.Length)//完整帧
                                {
                                    Data = BinFile.Skip(fileBlackSize * ID).Take(fileBlackSize).ToArray();
                                }else
                                    if ((fileBlackSize * ID) < BinFile.Length)//不足一帧
                                {
                                    Data = BinFile.Skip(fileBlackSize * ID).Take(BinFile.Length - fileBlackSize * ID).ToArray();
                                }
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UGC_ReadBinFile | 0x80, Data, Data.Length);
                                Value.WriteLog.WriteLine("Send:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                                break;
                            case (byte)UpGradeCmd.UGC_SetTextFile:
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UGC_SetTextFile | 0x80, null, 0);
                                Value.WriteLog.WriteLine("Send:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                                Value.WriteLog.WriteLine("LoadTextFile:" +ID , LogType.LT_Warning);
                                TextFile=Value.fileIO.ReadText(ID);break;
                            case (byte)UpGradeCmd.UGC_ReadTextFile:
                                byte[] Text=new byte[0];
                                if(ID<TextFile.Length)
                                {
                                    Text=System.Text.Encoding.Default.GetBytes(TextFile[ID]).Concat(new byte[]{0x00}).ToArray();
                                    Value.WriteLog.WriteLine("SendLine[" + ID + "]:" + TextFile[ID], LogType.LT_Warning);
                                }
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UGC_ReadTextFile | 0x80, Text, Text.Length);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                                break;
                            case (byte)UpGradeCmd.UGC_GetTextFileInfor:
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UGC_GetTextFileInfor | 0x80, Tools.ByteFromU32((UInt32)TextFile.Length, 0), 4);
                                Value.WriteLog.WriteLine("SendTextInfor:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());break;
                            case (byte)UpGradeCmd.UCG_GetDateTime:
                                byte[] TData=Tools.DateTimeToBytes(DateTime.Now);
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UCG_GetDateTime | 0x80, TData, TData.Length);
                                Value.WriteLog.WriteLine("SendDate:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());break;
                            case (byte)UpGradeCmd.UCG_SendBinCFG:
                                fileBlackSize = RxBuffer[3];
                                    fileBlackSize <<= 8;
                                    fileBlackSize += RxBuffer[4];
                                    BinCFG = BinCFG.Concat(RxBuffer.Skip(5).Take(fileBlackSize)).ToArray();
                                byte[] Bin=new byte[0];
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UCG_SendBinCFG | 0x80, Bin, Bin.Length);
                                Value.WriteLog.WriteLine("SendDate:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                Value.WriteLog.WriteLine("BinCFG[" + BinCFG.Length + "]:" + Tools.HexToString(BinCFG, BinCFG.Length), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());;
                                if (fileBlackSize == 0) Value.fileIO.WriteBinCFG(ID,BinCFG);
                                break;
                            case (byte)UpGradeCmd.UCG_SetBinCFGFile://设置焦点文件设置块大小
                                fileBlackSize = RxBuffer[5];
                                fileBlackSize <<= 8;
                                fileBlackSize += RxBuffer[6];
                                Value.WriteLog.WriteLine("Revice:" + fileBlackSize, LogType.LT_Warning);
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UCG_SetBinCFGFile | 0x80, null, 0);
                                Value.WriteLog.WriteLine("Send:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                                Value.WriteLog.WriteLine("LoadBinFile:" + ID, LogType.LT_Warning);
                                BinFile = Value.fileIO.ReadBinCFG(ID); break;
                            case (byte)UpGradeCmd.UCG_SetMenuList:
                                fileBlackSize = RxBuffer[3];
                                fileBlackSize <<= 8;
                                fileBlackSize += RxBuffer[4];
                                if (RxBuffer[0] == 0xff) 
                                {
                                   
                                    HK.Key = "aabbccdd";// Tools.ConvertTo(RxBuffer.Skip(5).Take(12).ToArray());
                                    if(Tools.HaseGetKey(HK.Key,Value.hs)==null)
                                    { 
                                    HK.Update = 0;
                                    HK.OnlyTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                    HK.Str = new string[RxBuffer[1]];
                                    Value.hs.Add(HK);
                                    }
                                }else
                                {
                                    Tools.HaseSetJoin(HK.Key, Value.hs,RxBuffer[1],Encoding.Default.GetString(RxBuffer, 5, fileBlackSize));
                                    HashKey hk1=Tools.HaseGetKey(HK.Key, Value.hs);
                                    if (hk1 != null)
                                    {
                                        hk1.Update = 0;
                                    }
                                }
                                Value.WriteLog.WriteLine("Revice:" + Encoding.Default.GetString(RxBuffer, 5, fileBlackSize), LogType.LT_Warning);
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UCG_SetMenuList | 0x80, null, 0);
                                Value.WriteLog.WriteLine("Send:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                               break;
                            case (byte)UpGradeCmd.UCG_GetMenuList:
                               string temp=Tools.GetHaseKey(HK.Key, Value.hs)[RxBuffer[1]];
                               if (temp.IndexOf(':') > 0)
                               {
                                   temp = temp.Substring(temp.IndexOf(':') + 1);
                                   BinCFG = Encoding.Default.GetBytes(temp);
                                   BinCFG=BinCFG.Concat(new byte[] { 0x00 }).ToArray();
                               }
                               else BinCFG = new byte[0];
                               RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UCG_GetMenuList | 0x80, BinCFG, BinCFG.Length);
                                Value.WriteLog.WriteLine("Send:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                               break;
                            case (byte)UpGradeCmd.UCG_CheckNewMenu:
                               HashKey hk = Tools.HaseGetKey(HK.Key, Value.hs);
                                BinCFG=new byte[0];
                                if(hk!=null)
                                {
                                    BinCFG = new byte[1] { hk.Update };
                                    hk.Update = 0;
                                }
                                RxLength = ZBUS.ZBUS_SendMsg(ref TxBuffer, ID, (byte)UpGradeCmd.UCG_GetMenuList | 0x80, BinCFG, BinCFG.Length);
                                Value.WriteLog.WriteLine("Send:" + Tools.HexToString(TxBuffer, RxLength), LogType.LT_Warning);
                                service.Send(TxBuffer.Skip(0).Take(RxLength).ToArray());
                                break;
                            default:break;
                        }

                    }
                    else { Value.WriteLog.WriteLine("校验失败:" + Tools.HexToString(RxBuffer, RxLength), LogType.LT_Warning); }
            }
            catch (Exception E)
            {
                Value.WriteLog.WriteLine("异常退出线程:"+E.Message, LogType.LT_Warning);
                try
                {
                    if (DevTrue) Tools.RemoveHK(HK.OnlyTime);
                    service.Close();
                }
                catch { Value.WriteLog.WriteLine("已断开", LogType.LT_Warning); }
                return;
            }
            } 
        }
    }
}
