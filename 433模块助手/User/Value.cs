using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _433模块助手
{
    public struct KeyMsgStruct
    {
        public  int Key_Value;
        public  int Key_Type;
    };
    class Value
    {
        public const int Key_Uping= 1;
        public const int Key_Downing = 2;
        public static bool App_Run = true;
        public static KeyMsgStruct KeyMsg = new KeyMsgStruct();
        public static Queue<KeyMsgStruct> KeyMsgQueue = new Queue<KeyMsgStruct>();
    }
}
