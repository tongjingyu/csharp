using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
namespace Geek_IDE
{
    class Value
    {
        public static string ComName;
        public static int COM_BaudRate;
        public static int DeviceAddr;
        public static byte[] UsartRxBuffer = new byte[100];
        public static bool UsartRxRealy = false;
        public static SerialPort Port1;
    }
}
