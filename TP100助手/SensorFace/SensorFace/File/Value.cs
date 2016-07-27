using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
namespace SensorFace
{
    class Value
    {
        public static bool App_Run = true;
        public static SerialPort serialPort1=new SerialPort();
        public static Queue<string> queue = new Queue<string>();
        public static List<byte[]> ReviceData = new List<byte[]>();
        public static bool UpdateBin = false;
    }
}
