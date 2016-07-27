using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 标准ModBus版
{
    public struct RS485Value
    {
       public float F_H_Value;//风机启控值
       public float T_C_Value;//超温报警值
       public float T_T_Value;//超温跳闸值
       public float SH1_Value;//湿度设定值
       public float SH2_Value;//湿度设定值
       public float _T_Value; //温度修正值
       public float SU_Value;//窗口选择设置
       public float TOP_Value;	//历史最高温度值
       public float RS485_Addr;//485通信地址
       public float Baud_Value;//波特率
       public float Oil_Temperature;
       public float Room_Temperature;
       public float Dampness_1;
       public float Dampness_2;
       public float PT100_ADC;
       public int Ctr;
       public int RS485Addr;
       public string Model;
    };
    class Value
    {
        public static bool App_Run = true;
        public static RS485Value RSValue=new RS485Value();
    }
}
