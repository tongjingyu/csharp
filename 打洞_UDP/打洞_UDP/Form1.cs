using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace 打洞_UDP
{
    public partial class Form1 : Form
    {
        bool APP_Run = true;
        Thread TaskListen;
        Thread TaskClient;
        string Msg="";
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Client();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TaskListen = new Thread(Listen);
            TaskClient = new Thread(Client);
            //TaskListen.Start();
        }
        private void Listen()
        {
            IPEndPoint MyIPAddress=new IPEndPoint(IPAddress.Parse("127.0.0.1"),222);
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ipa in ips)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    MyIPAddress=new IPEndPoint(ipa, 222);
                    break;
                }
            }
            UdpClient server = new UdpClient(MyIPAddress);
            while(APP_Run)
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                byte[] receiveBytes = server.Receive(ref remote);
                Msg = Encoding.GetEncoding("GB2312").GetString(receiveBytes);
                Msg += remote.Address.ToString();
                Msg += remote.Port.ToString();
                Thread.Sleep(100);
            }
        }
        private void Client()
        {
            
           // while (APP_Run)
            { 
                string sendString = null;//要发送的字符串 
                byte[] sendData = null;//要发送的字节数组 
                UdpClient client = null;
                IPAddress remoteIP = IPAddress.Parse("139.129.34.143");
                int remotePort =12345;
                IPEndPoint remotePoint = new IPEndPoint(remoteIP, remotePort);//实例化一个远程端点 
                sendString = "测试文字";
                sendData = Encoding.Default.GetBytes(sendString);
                client = new UdpClient(111);
                client.Send(sendData, sendData.Length, remotePoint);//将数据发送到远程端点 
                byte[] Data = client.Receive(ref remotePoint);
                Msg+="服务器地址:"+remotePoint.Address.ToString()+"端口:"+remotePoint.Port.ToString()+"\n";
                Msg +="消息:"+Encoding.Default.GetString(Data) + "\n";
                Thread.Sleep(100);
                client.Close();//关闭连接 
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (richTextBox1.Text == Msg) return;
            richTextBox1.Text = Msg;
            richTextBox1.Focus();
            richTextBox1.Select(richTextBox1.Text.Length, 0);
            richTextBox1.ScrollToCaret();
        }
    }
}
