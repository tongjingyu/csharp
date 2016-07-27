using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 字符编码
{
    class Tools
    {
        public static String HexToString(byte[] str, int Length)
        {
            string String = "";
            for (int i = 0; i < Length; i++)
            {
                String += str[i].ToString("X2");
                if (i < (Length - 1)) String += " ";
            }
            return String;
        }
        public static byte[] StringToHex(string s)
        {
            s = s.Replace(" ", "");
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
                }
                catch
                {

                }
            }
            return bytes;
        }
    }
}
