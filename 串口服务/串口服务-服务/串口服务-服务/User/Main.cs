using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
namespace 串口服务
{
    class MainThread
    {
        public static SerialPort Port1;
        public static void Serial1Server()
        {
            string[] SPNames = SerialPort.GetPortNames();
            Port1 = new SerialPort(SPNames[0], 115200);
            while (true)
            {
               // Port1.WriteLine("fdsaf");
                string aaa=  Console.ReadLine();
                if (aaa == "open")
                {
                    try { 
                    Port1.Open();
                        Console.WriteLine("Open Succeed!");
                    }
                    catch(Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }else 
                if (aaa == "close")
                {
                    Port1.Close();
                    Console.WriteLine("Close Succeed!");
                }
                else
                if (aaa == "ls")
                {
                    for (int i = 0; i < SPNames.Length; i++) Console.WriteLine(SPNames[i]);
                }else
                if(aaa=="help"|aaa=="")
                {
                    Console.WriteLine("open---------------(打开串口)");
                    Console.WriteLine("close--------------(关闭串口)");
                    Console.WriteLine("ls-----------------(扫描串口)");
                    Console.WriteLine("help---------------(帮助)");
                }else Console.WriteLine("cmd Error");
                System.Threading.Thread.Sleep(100);
            }
            
        }
    }
}
