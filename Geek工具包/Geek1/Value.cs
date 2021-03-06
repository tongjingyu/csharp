﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
namespace Geek1
{
    class Value
    {
        public struct CheckInfor
        {
            public byte Status;
            public string 电量;
            public string MAC;
            public string 版本;
            public string FlashID;
            public string Flash;
            public string GsensorID;
            public string Gsensor自检;
            public string 心率TIA;
            public string ADC;
            public string SN;
            public string 心率校验;
            public string 充电状态;
            public string 位号;
            public bool WriteOk;
            public string Date;
            public string COMName;
            public byte RISS;
        }

        public static string ComName;
        public static int WorkFlag=0;
        public static int COM_BaudRate;
        public static int DeviceAddr;
        public static byte[] UsartRxBuffer = new byte[100];
        public static bool UsartRxRealy = false;
        public static SerialPort Port1;
        public static bool Run;
        public static List<byte[]> ReviceData = new List<byte[]>();
        public static List<CheckInfor> strList = new List<CheckInfor>();
        public static string CurMAC;
        public static byte[] BLE搜索响应 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x00, 0x04, 0xFE, 0x00 };
        public static byte[] BLE搜索到设备 = { 0x04, 0xFF, 0x2C, 0x0D, 0x06, 0x00, 0x00, 0x00 };
        public static byte[] BLE初始化命令 = { 0x01, 0x00, 0xFE, 0x26, 0x08, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
        public static byte[] BLE获取参数1 = { 0x01, 0x31, 0xFE, 0x01, 0x15 };
        public static byte[] BLE获取参数2 = { 0x01, 0x31, 0xFE, 0x01, 0x16 };
        public static byte[] BLE获取参数3 = { 0x01, 0x31, 0xFE, 0x01, 0x1a };
        public static byte[] BLE获取参数4 = { 0x01, 0x31, 0xFE, 0x01, 0x19 };
        public static byte[] BLE初始化完毕 = { 0x04, 0xFF, 0x2C, 0x00, 0x06, 0x00, 0x27, 0xCB, 0x4B, 0x4C, 0x99, 0xB4, 0x1B, 0x00, 0x0C, 0x73 };
        public static byte[] BLE取消扫描 = { 0x01, 0x05, 0xFE, 0x00 };
        public static byte[] BLE操作忙 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x11, 0x04, 0xFE };
        public static byte[] BLE扫描完成 = { 0x04, 0xFF, 0x14, 0x01, 0x06, 0x00, 0x02, 0x00, 0x00, 0x0D, 0x47, 0x44, 0x10, 0x0F, 0xC8, 0x00 };
        public static byte[] BLE初始化成功 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x00, 0x00, 0xFE, 0x00, 0x04, 0xFF, 0x2C, 0x00, 0x06, 0x00, 0x27, 0xCB, 0x4B, 0x4C, 0x99, 0xB4, 0x1B, 0x00, 0x0C, 0x73, 0x41, 0x3B, 0xA4, 0x9F, 0xD2, 0x5D, 0x29, 0xD1, 0x3A, 0xF9, 0x5F, 0xD3, 0xC1, 0xD9, 0xDC, 0xF7, 0xB2, 0x73, 0x47, 0x3B, 0xA4, 0x5F, 0xD2, 0xA1, 0x79, 0x12, 0x3B, 0xC5, 0x0F, 0xFC, 0x50 };
        public static byte[] BLE获扫描结果 = { 0x04, 0xFF, 0x14, 0x01, 0x06, 0x00 };
        public static byte[] BLE已经执行该操作 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x11, 0x09, 0xFE, 0x00 };
        public static byte[] BLE配对响应 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x00, 0x09, 0xFE, 0x00 };
        public static byte[] BLE配对成功 = { 0x04, 0xFF, 0x13, 0x05, 0x06, 0x00, 0x00 };
        public static byte[] BLE断开连接 = { 0x04, 0xFF, 0x06, 0x06, 0x06, 0x00, 0x00, 0x00, 0x13 };
        public static byte[] BLE获取内容ff01 = { 0x01, 0xB4, 0xFD, 0x08, 0x00, 0x00, 0x01, 0x00, 0xFF, 0xFF, 0x01, 0xff };
        public static byte[] BLE获取内容ff0c = { 0x01, 0xB4, 0xFD, 0x08, 0x00, 0x00, 0x01, 0x00, 0xFF, 0xFF, 0x0c, 0xff };
        public static byte[] BLE获取内容ff03 = { 0x01, 0xB4, 0xFD, 0x08, 0x00, 0x00, 0x01, 0x00, 0xFF, 0xFF, 0x03, 0xff };
        public static byte[] BLE拒绝访问 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x02, 0xB4, 0xFD, 0x00 };
        public static byte[] BLE读取成功 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x00, 0xB4, 0xFD, 0x00 };
        public static byte[] BLE操作完成 = { 0x04,0xFF,0x06,0x09,0x05,0x1A,0x00,0x00,0x00};
        public static byte[] BLE未指命令 = { 0x04, 0xFF, 0x06, 0x06, 0x06, 0x00, 0x02, 0x00, 0x13 };
        public static byte[] BLE初始化 = { 0x04, 0xFF, 0x06, 0x7F, 0x06, 0x00, 0x00, 0xFE, 0x00 };
        public static byte[] BLE获取参数 = { 0x04,0xFF,0x08,0x7F,0x06,0x00,0x31,0xFE };
        public static byte[] BLE返回ff01 = { 0x04, 0xFF, 0x19, 0x09, 0x05, 0x00, 0x00, 0x00, 0x13, 0x12, 0x12, 0x00};
        public static byte[] BLE返回ff0c = { 0x04,0xFF,0x13,0x09,0x05,0x00,0x00,0x00,0x0D,0x0C,0x2C,0x00};
        public static byte[] BLE返回ff03 = { 0x04,0xFF,0x0A,0x09,0x05,0x00,0x00,0x00,0x04,0x03,0x16,0x00};
        public static byte[] BLE测试 = { 0x01, 0x92, 0xFD, 0x05, 0x00, 0x00, 0x2F, 0x00, 0x02};
        public static byte[] BLE获取参数完毕= { 0x04, 0xFF, 0x08, 0x7F, 0x06, 0x00, 0x31, 0xFE, 0x02, 0xD0, 0x07 };
        public static byte[] BLE测试完毕= { 0x04,0xFF,0x06,0x13,0x05,0x00,0x00,0x00,0x00};
        public static byte[] BLE断开设备 = { 0x01, 0x0A, 0xFE, 0x03, 0xFE, 0xFF, 0x13 };
        public static byte[] BLE取消搜索 = { 0x01, 0x05, 0xFE, 0x00 };
        public static byte[] BLE重复连接 = { 0x0, 0xFF, 0x06, 0x7F, 0x06, 0x11, 0x09, 0xFE, 0x00 };

        
        
        
    }
   
        
}
