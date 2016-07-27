using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
namespace Server
{
    class WebServer
    {
        public static TcpListener TcpLiserner;
        public void MainThread()
        {
            Value.Path = "C:\\TPServer";
            Value.CFG = new Configs(Value.Path);
            Value.WriteLog = new Log(Value.Path, LogType.LT_All);
            Value.CFG.Load();
            Value.fileIO = new FileIO(Value.Path);
            CreateServiceThread();
            while (TagetValue.Run)
            {
                Thread.Sleep(1000);
                break;
            }

        }
        public static bool CreateServiceThread()
        {
            int Count = 0;
            while (TagetValue.Run && Count++ < 1000)
            {
                try
                {
                    TcpLiserner = new TcpListener(IPAddress.Any, Value.ServerPort);
                    TcpLiserner.Start();
                    break;
                }
                catch { System.Threading.Thread.Sleep(100); }
            }

            Value.WriteLog.WriteLine("监听ALL[" + Value.ServerPort + "]", LogType.LT_Warning);
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
                    Socket client = TcpLiserner.AcceptSocket();//当有可用的客户端连接尝试时执行，并返回一个新的socket,用于与客户端之间的通信
                    WebClientThread newclient = new WebClientThread(client);
                    IPEndPoint clientip = (IPEndPoint)client.RemoteEndPoint;
                    Value.WriteLog.WriteLine("new Client:" + clientip.Address + ":" + clientip.Port, LogType.LT_Warning);
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

        class WebClientThread
        {
            public static int connections = 0;
            public Socket service;
            public WebClientThread(Socket clientsocket)
            {
                this.service = clientsocket;
            }
            private Encoding charEncoder = Encoding.UTF8; // To encode string
            private void sendButtonResponse(Socket clientSocket)
            {
                string msg = "HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nPragma: no-cache\r\n\r\n" +
                "<title>TP配置</title>" +
                 "<style> " +
                ".divcss5-cent{margin:0 auto;width:800px;height:auto;border:0px solid #bbb} " +
                "</style> " +
                "<html>" +
                "<head>" +
                "<body style=\"background-color:#eee\">" +
                "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" +
                "<P align=center><STRONG><FONT size=7 color=#008000 ><SPAN id=wiz_font_size_span_285640406 style=\"FONT-SIZE: 40pt\">" +
                "设备状态：在线\r\n" +
                "</FONT></STRONG></P>" +
                "<HR color=#ff0000>" +
                "<form  method=\"post\">" +
                 "<div class=\"divcss5-cent\">" +
                 Html.AddTextBoxLable("TextBox1", null, "短信通知", 30) +
                 Html.AddTextBoxLable("TextBox2", "12", "主机地址", 30) +
                 Html.AddTextBoxLable("TextBox3", "12", "主机端口", 30) +
                 Html.AddTextBoxLable("TextBox4", "12", "温度上限", 30) +
                 Html.AddTextBoxLable("TextBox5", "12", "温度下限", 30) +
                 Html.AddTextBoxLable("TextBox6", "12", "湿度上限", 30) +
                 Html.AddTextBoxLable("TextBox7", "12", "湿度下限", 30) +
                 Html.AddTextBoxLable("TextBox8", "12", "上传周期", 30) +
                 Html.AddTextBoxLable("TextBox9", "12", "短信通知", 30) +
                 Html.AddrTwoButton("检查", "提交", 30)+
                "</div> " +
                "</form>" +
                "</html>";
                byte[] bHeader = charEncoder.GetBytes(msg);
                Value.WriteLog.WriteLine("发送:" + msg, LogType.LT_Warning);
                clientSocket.Send(bHeader);
            }
            public void handleTheRequest(Socket clientSocket)
            {
                int i = 0;
                byte[] buffer = new byte[10240];
                int receivedBCount = clientSocket.Receive(buffer);
                string strReceived = charEncoder.GetString(buffer, 0, receivedBCount);
                Value.WriteLog.WriteLine("接收:" + strReceived, LogType.LT_Warning);
                if(strReceived.IndexOf("GET")>-1)
                {
                    sendButtonResponse(clientSocket);
                    clientSocket.Close();
                    return;
                }
                if (strReceived.IndexOf("POST") > -1)
                {
                    if (strReceived.IndexOf("ButtonSubmit") < 0)
                    {
                        receivedBCount = clientSocket.Receive(buffer);
                        strReceived = charEncoder.GetString(buffer, 0, receivedBCount);
                        Value.WriteLog.WriteLine("接收2:" + strReceived, LogType.LT_Warning);
                    }
                    sendButtonResponse(clientSocket);
                    clientSocket.Close();
                    return;
                }

               
            }

            public void ClientService()
            {
                byte[] RxBuffer = new byte[1500];
                byte[] TxBuffer = new byte[1500];
                byte[] BinFile = new byte[0];
                string[] TextFile = new string[0];
                byte[] BinCFG = new byte[0];
                Value.DevOnlineCount++;
                service.ReceiveTimeout = 80000;
                Value.WriteLog.WriteLine("设备接入", LogType.LT_Warning);
                try{
                  handleTheRequest(service);
                }
                catch (Exception E)
                {
                    Value.WriteLog.WriteLine("Main2" + E.Message, LogType.LT_Warning);
                    return;
                }
                Value.WriteLog.WriteLine("设备断开", LogType.LT_Warning);
            }
        }
    }

