using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace 杭州升降机模拟设备
{
    class TCPIP
    {
        public IPEndPoint IpPoint;
        TcpClient Client;
        NetworkStream SendStream;
        public TCPIP(IPEndPoint IpPoint)
        {
            this.IpPoint = IpPoint;
        }
        public void Connect()
        {
            Client = new TcpClient();
            Client.SendTimeout = 1000;
            Client.Connect(IpPoint);
            SendStream = Client.GetStream();
            Client.ReceiveTimeout = 1000;
        }
        public void Write(byte[] Buffer,int Length)
        {
            SendStream.Write(Buffer,0, Length);
        }
        public byte[] Read()
        {
            byte[] Buf = new byte[2048];
            Client.ReceiveTimeout = 10000;
           int Length=SendStream.Read(Buf, 0, 100);
            byte[] ReBuf = new byte[Length];
            for (int i = 0; i < Length; i++) ReBuf[i] = Buf[i];
            return ReBuf;
        }

    }
}
