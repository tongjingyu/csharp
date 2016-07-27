using System;
using System.Collections.Generic;
using System.Text;

namespace DTU配置工具V1._0
{
    class CheckTools
    {

        //======================生成16位CRC校验码=====================================
        public static uint GetCrc16(byte[] DataArray, int DataLenth)
        {
            uint i, j, MyCRC = 0xFFFF;
            for (i = 0; i < DataLenth; i++)
            {
                MyCRC = (MyCRC & 0xFF00) | ((MyCRC & 0x00FF) ^ DataArray[i]);
                for (j = 1; j <= 8; j++)
                {
                    if ((MyCRC & 0x01) == 1) { MyCRC = (MyCRC >> 1); MyCRC ^= 0XA001; } else { MyCRC = (MyCRC >> 1); }
                }
            }
            return MyCRC;
        }

        //===============数据纵向和校验(字符串型)===========================
        public static string CheckSum(string TempStr)
        {
            char[] Temp = TempStr.ToCharArray();
            int c = 0;
            for (int a = 0; a < Temp.Length; a++)
            {
                c += Temp[a];
            }
            c %= 256;
            return c.ToString("X");
        }

        //===============数据纵向和校验(字节数组型)===========================
        public static byte CheckSum(byte[] TempData, int Len)
        {
            int c = 0;
            for (int a = 0; a < Len; a++)
            {
                c += TempData[a];
            }
            c %= 256;
            return (byte)c;
        }

        //===============数据异或校验========================================
        public static byte CheckXor(string TempStr)
        {
            byte[] Temp = System.Text.ASCIIEncoding.ASCII.GetBytes(TempStr);
            byte Re = 0;
            for (int i = 0; i < Temp.Length; i++)
            {
                Re = (byte)(Re ^ Temp[i]);
            }
            return Re;
        }



        //----------------得到MD5加密码------------------------------------------------
        public static string GetMD5(string str)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
            {
                ret += b[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }

        //----------------反转字符串----------------------------------------------------
        public static string Reverse(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        //------用户名的解码算法-----------------------------
        //说明:
        //反转字符串,奇字符大小写反转,MD5加密
        //约束,串长度在6-20之间
        public static string Get_Pid(string Ts)
        {
            string MyPwd = "";
            if ((Ts.Length < 5) || (Ts.Length > 20)) return "";
            else
            {
                Ts = Reverse(Ts);
                for (int i = 0; i < Ts.Length; i += 2)
                {
                    string Ks = Ts.Substring(i, 1);
                    if (Ks == Ks.ToUpper())
                    {
                        MyPwd += Ks.ToLower();
                    }
                    else
                    {
                        MyPwd += Ks.ToUpper();
                    }
                }
                MyPwd = GetMD5(MyPwd);
            }
            return MyPwd;
        }

        //------密码的解码算法-----------------------------
        //说明:
        //反转字符串,偶字符大小写反转,MD5加密
        //约束,串长度在6-20之间
        public static string Get_Pwd(string Ts)
        {
            string MyPwd = "";
            if ((Ts.Length < 5) || (Ts.Length > 20)) return "";
            else
            {
                Ts = Reverse(Ts);
                for (int i = 1; i < Ts.Length; i += 2)
                {
                    string Ks = Ts.Substring(i, 1);
                    if (Ks == Ks.ToUpper())
                    {
                        MyPwd += Ks.ToLower();
                    }
                    else
                    {
                        MyPwd += Ks.ToUpper();
                    }
                }
                MyPwd = GetMD5(MyPwd);
            }
            return MyPwd;
        }


        //时间解码
        public static DateTime RTC_DecodeTime(int TimeData)
        {
            int T, K, B;
            int T16, K16;
            int Year, Month, Day, Hour, Min, Sec;
            K16 = TimeData % 86400;		        	        //分离时/分/秒   
            T16 = TimeData / 86400;     		            //分离年/月/日   
            Sec = K16 % 60;                  		//计算秒   
            K16 /= 60; Min = K16 % 60;               	//计算分   
            K16 /= 60; Hour = K16 % 24;             		//计算时
            //Week = (T16 + 6) % 7;							//根据总天数计算星期
            for (K16 = 0, T = 0; T < 150; T++)
            {  		            //按天累计年份   
                K16 = (T & 0x03) > 0 ? 365 : 366;       	        //统计闰年 		
                if (T16 < K16) { K16 -= 337; break; }    	    //计算当年二月份虚天数
                else T16 -= K16;                  		//减掉年份    
            }
            Year = T;                      			//计算得到相对年份   
            for (T = 1; T < 13; T++)
            {
                if (((T + (T >> 3)) & 0x01) > 0) B = 31;
                else B = 30;
                K = (T == 2) ? K16 : B;    //统计当月天数
                if (T16 >= K) T16 -= K;             			//减掉已经足够的月份		
                else break;
            }
            Month = T;                     			//计算得到当年中的月份   
            Day = T16 + 1;                       		//计算得到当年中的天   
            DateTime DT = new DateTime(Year + 2000, Month, Day, Hour, Min, Sec);
            return DT;
        }


        //时间编码
        public static int RTC_EncodeTime(DateTime DT)
        {
            int T, K;
            int T32, TimeData = 0;
            for (T = 0, K = 0; T < (DT.Year - 2000); T++, K = T & 0x03)
            {				//累加年份				
                TimeData += K > 0 ? 31536000 : 31622400;
            }
            for (T = 1; T < DT.Month; T++)
            {
                if (T != 2)
                {										//统计平月
                    T32 = (((T + (T >> 3)) & 0x01) > 0) ? 2678400 : 2592000;
                }
                else T32 = K > 0 ? 2419200 : 2505600;					//统计闰月
                TimeData += T32;
            }
            T32 = DT.Day - 1; TimeData += T32 * 86400;					//统计天
            T32 = DT.Hour; TimeData += T32 * 3600;					//统计时
            T32 = DT.Minute; TimeData += T32 * 60;					//统计分
            TimeData += DT.Second;								//统计秒
            return TimeData;
        }


        //获取D16数据
        public static int DAT_GetD16FromArray(byte[] Datas, int Offset, int Mode)
        {
            int T = 0;
            if (Mode == 0)
            {        //小端格式
                T = Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset];
            }
            else if (Mode == 1)
            {   //大端格式
                T = Datas[Offset]; T <<= 8;
                T |= Datas[Offset];
            }
            return T;
        }

        //获取D32数据
        public static int DAT_GetD32FromArray(byte[] Datas, int Offset, int Mode)
        {
            int T = 0;
            if (Mode == 0)
            {    //小端格式
                T = Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset];
            }
            else if (Mode == 1)
            {   //大端格式
                T = Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 3];
            }
            else if (Mode == 2)
            {
                T = Datas[Offset + 1]; T <<= 8;
                T |= Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset + 2];
            }
            else if (Mode == 3)
            {
                T = Datas[Offset + 2]; T <<= 8;
                T |= Datas[Offset + 3]; T <<= 8;
                T |= Datas[Offset]; T <<= 8;
                T |= Datas[Offset + 1];
            }
            return T;
        }

    }
}
