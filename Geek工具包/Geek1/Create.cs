using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geek1
{
    class Create
    {
        public static Value.CheckInfor CreateInfor(string mac, string 版本, string 电量, byte Char, string 位号)
        {
            Value.CheckInfor CI = new Value.CheckInfor();
            CI.MAC = mac;
            CI.电量 = 电量;
            CI.版本 = 版本;
            CI.位号 = 位号;
            if ((Char & (byte)(1 << 0)) > 0) CI.FlashID = "错误"; else CI.FlashID = "正确";
            if ((Char & (byte)(1 << 1)) > 0) CI.Flash = "错误"; else CI.Flash = "正确";
            if ((Char & (byte)(1 << 2)) > 0) CI.GsensorID = "错误"; else CI.GsensorID = "正确";
            if ((Char & (byte)(1 << 3)) > 0) CI.Gsensor自检 = "错误"; else CI.Gsensor自检 = "正确";
            if ((Char & (byte)(1 << 4)) > 0) CI.心率TIA = "错误"; else CI.心率TIA = "正确";
            if ((Char & (byte)(1 << 5)) > 0) CI.ADC = "错误"; else CI.ADC = "正确";
            if ((Char & (byte)(1 << 6)) > 0) CI.心率校验 = "错误"; else CI.心率校验 = "正确";
            if ((Char & (byte)(1 << 7)) > 0) CI.充电状态 = "连接"; else CI.充电状态 = "断开";
            return CI;
        }
    }
}
