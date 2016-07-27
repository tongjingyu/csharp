using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharp
{
     struct Configs
    {
	public float F_H_Value;//风机启控值
    public float F_C_Value;//风机控制回差值
    public float T_C_Value;//超温报警值
    public float T_T_Value;//超温跳闸值
    public float SH2_Value;//湿度设定值
    public float _T_Value; //温度修正值
    public float SU_Value;//窗口选择设置
    public float TOP_Value;	//历史最高温度值
    public float RS485_Addr;//485通信地址
    public float Baud_Value;//波特率
    public float Correct_TEMP_A;//校准A点温度
    public float Correct_ADC_A;//校准A点模数
    public float Correct_TEMP_B;//校准B点温度
    public float Correct_ADC_B;//校准B点模数
    public float Correct_04_A;//4-20毫安的4毫安对应的pwm值
    public float Correct_20_B;//4-20毫安的20毫安对应的pwm值
     }
    /*Configs CFGDef={
		45.0,
		5.0,
		85.0,
		100.0,
		85.0,
		0.0,
		2,
		0,
		2,
		3, //波特率
		20.0,
		300,
		30.0,
		600,
		400,
		2000
		};*/
    class App
    {
        public static void SetDefValue(ref Configs CFG)
        {
            CFG.F_H_Value=(float)45.0;
            CFG.F_C_Value = (float)5.0;
            CFG.T_C_Value = (float)85.0;
            CFG.T_T_Value = (float)100;
            CFG.SH2_Value = (float)85;
            CFG._T_Value = (float)0;
            CFG.SU_Value = (float)2;
            CFG.TOP_Value = (float)0;
            CFG.RS485_Addr = (float)2;
            CFG.Baud_Value = (float)3;
        }

    }
}
