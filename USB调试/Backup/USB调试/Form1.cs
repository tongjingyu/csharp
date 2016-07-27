using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.LibUsb;

namespace USB调试
{
    public partial class Form1 : Form
    {
        const int myPID = 0x0101;  //产品ID
        const int myVID = 0x1366;  //供应商ID

        public static UsbDevice MyUsbDevice;//USB设备
        public static DeviceNotifier DeviceNotifier = new DeviceNotifier();//设备变化通知函数
        public static UsbEndpointWriter writer = null;
        public static UsbEndpointReader reader = null;

        delegate void SetTextCallback(string text);//安全线程访问txtReadInt的值


        Boolean EnbaleInt = false;//是否使用中断接收

        public Form1()
        {
            InitializeComponent();
        }
        private void ShowCon(string msg)
        {
          label1.Text = msg;
        }

        private void ShowMsg(string msg)
        {
            label1.Text = msg;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (FindAndOpenUSB(myVID, myPID) == true)
                ShowCon("设备已连接");
            else
                ShowCon("设备未连接");

            //DeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;

            //writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep03);
            //reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep02);

            //if (EnbaleInt == true)
            //{
            //    reader.DataReceived += (OnRxEndPointData);
            //    reader.DataReceivedEnabled = true;
            //}

        }
        private bool FindAndOpenUSB(int PID, int VID)
         {
             UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(PID, VID);
             UsbRegistry myUsbRegistry   = UsbGlobals.AllDevices.Find(MyUsbFinder);

             if (ReferenceEquals(myUsbRegistry, null))
             {
                 return false;
             }
             // Open this usb device.
            if (!myUsbRegistry.Open(out MyUsbDevice))
             {
                 return false;
             }

             MyUsbDevice.SetConfiguration(1);

             ((LibUsbDevice)MyUsbDevice).ClaimInterface(0);

             ShowMsg(string.Format("Find Device:{0}",myUsbRegistry[SPDRP.DeviceDesc]));
             return true;
         }
         //关闭USB设备
        public void CloseUSB()
         {
             if (!ReferenceEquals(reader, null))
                 reader.Dispose();
             if (!ReferenceEquals(writer, null))
                 writer.Dispose();
             if (!ReferenceEquals(MyUsbDevice,null))
                 MyUsbDevice.Close();
         }
         //获得上次错误信息
        public string GetLastError()
         {
             return UsbGlobals.LastErrorString;
         }
         //设备变化消息相应函数
        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
         {
             if (e.EventType == EventType.DeviceArrival)
             {
                 ShowMsg(string.Format("发现有新USB设备连接，PID = 0x{0:X},VID = 0x{1:X}.\r\n设备的详细信息{2}", e.Device.IdProduct, e.Device.IdVendor, e.Device.ToString()));
                 //看看目前新连接的USB设备是不是目标设备
                if (e.Device.IdProduct == myPID && e.Device.IdVendor == myVID)
                 {
                     ShowMsg("该USB设备是目标设备");
                     //发现目标设备并打开该设备
                    FindAndOpenUSB(myPID,myVID);
                 }
                 else
                 {
                     ShowMsg("该USB设备不是目标设备\r\n");
                 }
             }
             else if (e.EventType == EventType.DeviceRemoveComplete)
             {

                 ShowMsg(string.Format("发现有USB设备移除，PID = 0x{0:X}, VID = 0x{1:X}\r\n设备的详细信息{2}", e.Device.IdProduct, e.Device.IdVendor, e.Device.ToString()));
                 //看看目前移除的USB设备是不是目标设备
                if (e.Device.IdProduct == myPID && e.Device.IdVendor == myVID)
                 {
                     ShowMsg(string.Format("移除的USB设备是目标设备\r\n"));
                     CloseUSB();
                 }
                 else
                 {
                     ShowMsg(string.Format("移除的USB设备不是目标设备\r\n"));
                 }
             }
         }
         //USB中断接收函数
        private void OnRxEndPointData(object sender, EndpointDataEventArgs e)
         {
            //txtReadInt.Text = Encoding.Default.GetString(e.Buffer, 0, e.Count);
             //MessageBox.Show(Encoding.Default.GetString(e.Buffer, 0, e.Count));
            SetText(Encoding.Default.GetString(e.Buffer, 0, e.Count));
         }


         private void btnSend_Click(object sender, EventArgs e)
         {
             ErrorCode ec = ErrorCode.None;
             
             int bytesWritten;
             try
             {
                 ec = writer.Write(Encoding.Default.GetBytes(textBox1.Text), 2000, out bytesWritten);
                 if (ec != ErrorCode.None)
                     throw new Exception(UsbGlobals.LastErrorString);
             }
             catch (Exception ex)
             {
                 ShowMsg("Error:" + ex.Message);
             }
             finally
             {
                
             }
         }

         private void btnRead_Click(object sender, EventArgs e)
         {
             ErrorCode ec = ErrorCode.None;
            
             byte[] readBuffer = new byte[1024];
             int bytesRead;
             try
             {
                 ec = reader.Read(readBuffer, 100, out bytesRead);
                 if (bytesRead == 0)
                     throw new Exception("No more bytes!");
                 textBox2.Text = Encoding.Default.GetString(readBuffer, 0, bytesRead);
             }
             catch (Exception ex)
             {
                 ShowMsg("Error:" + ex.Message);
             }
             finally
             {
                
             }
         }
         //线程安全访问txtReadInt
        private void SetText(string text)
         {
             // InvokeRequired required compares the thread ID of the
             // calling thread to the thread ID of the creating thread.
             // If these threads are different, it returns true.
           // if (this.txtReadInt.InvokeRequired)
             {
                 SetTextCallback d = new SetTextCallback(SetText);
                 this.Invoke(d, new object[] { text });
             }
           //  else
             {
            //     this.txtReadInt.Text = text;
             }
         }
        private void button1_Click(object sender, EventArgs e)
        {
            int VID=0,PID=0;
            GetDriverVersion("disk");
            for(int n=0;n<0xffff;n++)
            {
                for(int m=0;m<0xffff;m++)
                {
                    if (FindAndOpenUSB(VID, PID) == true) MessageBox.Show("VID" + VID.ToString() + "PID" + PID.ToString());

                }
            }
        }

    }
}
