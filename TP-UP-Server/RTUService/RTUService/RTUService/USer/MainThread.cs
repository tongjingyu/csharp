using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;
using System.Collections;
using System.IO;

namespace RTUService
{
    class ClientThread
    {
        public static int connections = 0;
        public Socket service;
        public ClientThread(Socket clientsocket)
        {
            this.service = clientsocket;
        }
        public static bool CreateServiceThread()
        {
            int Count = 0;
            while (SysFlag.ServiceStart && Count++ < 1000)
            {
                try
                {
                    Configs.ListenSock.Bind(Tools.Get_IpPoint(Configs.ServicePoint));//绑定
                    break;
                }
                catch { System.Threading.Thread.Sleep(100); }
            }
            Configs.ListenSock.Listen(10);//监听
            Configs.ListenSock.ReceiveTimeout = 5000;
            Configs.ListenSock.SendTimeout = 5000;
            SysFlag.ListenOk = true;
            if (Count > 990)
            {
                CreateInfor.WriteLogs("【申请用户交互端口失败】请释放或更改端口!");
                SysFlag.ListenOk = false;
                SysFlag.ServiceStart = false;//服务启动失败
                return false;
            }
            CreateInfor.WriteLogs("第【" + Count.ToString() + "】次申请侦听端口才成功!");
            Thread ServiceThread = new Thread(new ThreadStart(ServiceMainThread));
            ServiceThread.Start();
            CreateInfor.WriteLogs("侦听主线程创建成功!使用通道【"+Tools.Get_IpPoint(Configs.ServicePoint).ToString()+"】");
            return true;
        }
        private static void ServiceMainThread()
        {
            MainThread();
        }
        public static void MainThread()
        {
            while (SysFlag.ServiceStart)
            {
                try
                {
                    Socket client = Configs.ListenSock.Accept();//当有可用的客户端连接尝试时执行，并返回一个新的socket,用于与客户端之间的通信
                    IPEndPoint clientip = (IPEndPoint)client.RemoteEndPoint;
                    ClientThread newclient = new ClientThread(client);
                    Thread newthread = new Thread(new ThreadStart(newclient.ClientService));
                    newthread.Start();
                    SysFlag.ListenOk = true;
                    if (!SysFlag.ServiceStart) { client.Close(); return; }
                }
                catch (Exception E)
                {
                    if(SysFlag.ListenOk) CreateInfor.WriteLogs(E.Message+"导致【RTU服务主线程抛锚】!");
                    SysFlag. ListenOk = false;
                }
            }
            Configs.ListenSock.Close();
            CreateInfor.WriteLogs("侦听通道关闭!");
        }
        public void ClientService()
        {
            byte[] bytes = new byte[Configs.DataByteSize];
            byte[] sendbytes = new byte[Configs.DataByteSize];
            if (service != null)
            {
                SysFlag.ClientCount++;
            }
            try
            {
                NetworkStream NewStream = new NetworkStream(service);
                NewStream.ReadTimeout = Configs.ReviceTimeOut;
                while (SysFlag.ServiceStart)
                {
                    try
                    {
                        //int RxLength = NewStream.Read(bytes, 0, Configs.DataByteSize);
                        int RxLength = service.Receive(bytes);
                        if (RxLength > Configs.AgreementAAAMinLength)
                        {
                            int TxLength = 0;
                            bool ok = DataCollection.ExpendRecord(bytes, RxLength);
                            if (ok) TxLength = DataCollection.CreateReturnInfor(ref sendbytes, "OK");
                            else TxLength = DataCollection.CreateReturnInfor(ref sendbytes, "ER");
                            //NewStream.Write(sendbytes, 0, TxLength);
                            service.Send(sendbytes);
                        }
                        else break;
                    }
                    catch (Exception E)
                    {
                        CreateInfor.WriteLogs(E.Message + "导致【RTU服务子线程抛锚】!");
                        break;
                    }

                }service.Close();
            }
            catch { }
            if(SysFlag.ClientCount>0)SysFlag.ClientCount--;
        }
    }
}
