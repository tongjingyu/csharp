using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


namespace DTU配置工具V1._0
{
    public class SysConfig
    {
        #region 定义系统配置结构
        public struct ConfigParams
        {
            //通讯端口配置 CommPort
            public string CommPortname;
            public int CommBaudrate;
            public int Address;  
            //实时数据端口配置 DeviceConfig
            public DateTime ProductDate;
            public int DevNumber;
            public int PasswordCode;
            public string CompName;
            public string StationName;

            //public string ObjFilepath;  //文件名

            //CommParams
            public float WarningLevel;
            public int WarnInterval;
            public bool WarnEnable;
            public int BaseLevel;
            public int IncreaseLevel;
            public int InterTimeSet;
            //DTU配置 DTUConfig
            public int Authcode;
            public int TelCount;
            public string TelNum1;
            public string TelNum2;
            public string TelNum3;
            public string TelNum4;
            public string TelNum5;
            public string IpAddr1;
            public string IpAddr2;
            public string IpAddr3;
            public string IpAddr4;
            public string IpAddr5;
            public string IpPort1;
            public string IpPort2;
            public string IpPort3;
            public string IpPort4;
            public string IpPort5;
           
            
        }
        #endregion

        public ConfigParams Config = new ConfigParams();
        public XmlDocument XmlDoc;
        private string FilePath;

        //public static SortedList<UInt32, McuDeviceList> DeviceList = new SortedList<UInt32, McuDeviceList>();

        #region 读取通讯配置参数
        private void Xml_GetCommPort(XmlNode TempNode)
        {
            if (TempNode != null)
            {
                foreach (XmlNode CurNote in TempNode)
                {
                    switch (CurNote.Name.ToUpper())
                    {
                        case "PORTNAME": Config.CommPortname = CurNote.InnerText; break;
                        case "BAUDRATE": Config.CommBaudrate = int.Parse(CurNote.InnerText); break;
                        case "ADDRESS": Config.Address = int.Parse(CurNote.InnerText); break;
                    }
                }
            }
        }
        #endregion

        #region 实时数据配置参数
        private void Xml_GetRealTimeParmas(XmlNode TempNode)
        {
            if (TempNode != null)
            {
                foreach (XmlNode CurNote in TempNode)
                {
                    switch (CurNote.Name.ToUpper())
                    {
                        case "PRODUCTDATE": Config.ProductDate = DateTime.Parse(CurNote.InnerText); break;
                        case "DEVNUMBER": Config.DevNumber = int.Parse(CurNote.InnerText); break;
                        case "PASSWORDCODE": Config.PasswordCode = int.Parse(CurNote.InnerText); break;
                        case "COMPNAME": Config.CompName = CurNote.InnerText; break;
                        case "STATIONNAME": Config.StationName = CurNote.InnerText; break;
                        //case "OBJFILEPATH": Config.ObjFilepath = CurNote.InnerText; break;
                    }
                }
            }
        }
       #endregion

        //#region 密集数据配置参数
        //private void Xml_GetDensityParams(XmlNode TempNode)
        //{
        //    if (TempNode != null)
        //    {
        //        foreach (XmlNode CurNote in TempNode)
        //        {
        //            switch (CurNote.Name.ToUpper())
        //            {                       
        //                case "DS_DEVNUMBER": Config.DS_DevNumber = int.Parse(CurNote.InnerText); break;
        //                case "DS_PASSWORD": Config.DS_Password = int.Parse(CurNote.InnerText); break;                       
        //            }
        //        }
        //    }
        //}
        //#endregion

        #region CommParams
        private void Xml_GetCommParmas(XmlNode TempNode)
        {
            if (TempNode != null)
            {
                foreach (XmlNode CurNote in TempNode)
                {
                    switch (CurNote.Name.ToUpper())
                    {
                        case "WARNINGLEVEL": Config.WarningLevel = int.Parse(CurNote.InnerText); break;
                        case "WARNINTERVAL": Config.WarnInterval = int.Parse(CurNote.InnerText); break;
                        case "WARNENABLE": Config.WarnEnable = (CurNote.InnerText == "0") ? false : true; break;
                        case "BASELEVEL": Config.BaseLevel = int.Parse(CurNote.InnerText); break;
                        case "INCREASELEVEL": Config.IncreaseLevel = int.Parse(CurNote.InnerText); break;
                        case "INTERTIMESET": Config.InterTimeSet = int.Parse(CurNote.InnerText); break;
                       
                    }
                }
            }
        }
        #endregion 

        #region DTU设备参数
        private void Xml_GetDTUParams(XmlNode TempNode)
        {
            if (TempNode != null)
            {
                foreach (XmlNode CurNote in TempNode)
                {
                    switch (CurNote.Name.ToUpper())
                    {
                        case "AUTHCODE": Config.Authcode = int.Parse(CurNote.InnerText); break;
                        case "TELCOUNT": Config.TelCount = int.Parse(CurNote.InnerText); break;
                        case "TELNUM1": Config.TelNum1 = CurNote.InnerText; break;
                        case "TELNUM2": Config.TelNum2 = CurNote.InnerText; break;
                        case "TELNUM3": Config.TelNum3 = CurNote.InnerText; break;
                        case "TELNUM4": Config.TelNum4 = CurNote.InnerText; break;
                        case "TELNUM5": Config.TelNum5 = CurNote.InnerText; break;
                        case "IPADDR1": Config.IpAddr1 = CurNote.InnerText; break;
                        case "IPADDR2": Config.IpAddr2 = CurNote.InnerText; break;
                        case "IPADDR3": Config.IpAddr3 = CurNote.InnerText; break;
                        case "IPADDR4": Config.IpAddr4 = CurNote.InnerText; break;
                        case "IPADDR5": Config.IpAddr5 = CurNote.InnerText; break;
                        case "IPPORT1": Config.IpPort1 = CurNote.InnerText; break;
                        case "IPPORT2": Config.IpPort2 = CurNote.InnerText; break;
                        case "IPPORT3": Config.IpPort3 = CurNote.InnerText; break;
                        case "IPPORT4": Config.IpPort4 = CurNote.InnerText; break;
                        case "IPPORT5": Config.IpPort5 = CurNote.InnerText; break;
                                         
                    }
                }
            }
        }
        #endregion 
       
        #region 读XML中的内容
        private void Xml_ReadContent(XmlNode StartNode,int ConfigType)
        {
            if (StartNode != null)
            {
                foreach (XmlNode TempNode in StartNode.ChildNodes)
                {
                    switch (TempNode.Name.ToUpper())
                    {
                        case "COMMPORT": if ((ConfigType & 0x01) > 0) Xml_GetCommPort(TempNode); break;
                        case "DEVICEPARAMS": if ((ConfigType & 0x08) > 0) Xml_GetRealTimeParmas(TempNode); break;
                        case "COMMPARAMS": if ((ConfigType & 0x08) > 0) Xml_GetCommParmas(TempNode); break;
                        case "DTUPARAMS": if ((ConfigType & 0x08) > 0) Xml_GetDTUParams(TempNode); break;
                        default: break;
                    }
                }
            }
        }
        #endregion

        #region 保存配置文件
        public bool SaveConfig(int ConfigType)
        {
            bool R = true;
            try
            {                
                XmlNode TempNode = XmlDoc.FirstChild;
                XmlNode CurNote;
                TempNode = TempNode.NextSibling;
              
                switch (ConfigType)
                {
                    case 0:     //commconfig
                        TempNode = TempNode.SelectSingleNode("CommPort");
                        CurNote = TempNode.SelectSingleNode("Portname");
                        CurNote.InnerText = Config.CommPortname;
                        CurNote = TempNode.SelectSingleNode("Baudrate");
                        CurNote.InnerText = Config.CommBaudrate.ToString();
                        CurNote = TempNode.SelectSingleNode("Address");
                        CurNote.InnerText = Config.Address.ToString();
                        break;
                    case 1:
                        TempNode = TempNode.SelectSingleNode("DeviceParams");
                        CurNote = TempNode.SelectSingleNode("ProductDate");
                        CurNote.InnerText = Config.ProductDate.ToString();
                        CurNote = TempNode.SelectSingleNode("DevNumber");
                        CurNote.InnerText = Config.DevNumber.ToString();
                        CurNote = TempNode.SelectSingleNode("PasswordCode");
                        CurNote.InnerText = Config.PasswordCode.ToString();
                        CurNote = TempNode.SelectSingleNode("CompName");
                        CurNote.InnerText = Config.CompName;
                        CurNote = TempNode.SelectSingleNode("StationName");
                        CurNote.InnerText = Config.StationName;

                        //CurNote = TempNode.SelectSingleNode("ObjFilepath");
                        //CurNote.InnerText = Config.ObjFilepath;
                        break;
                 
                    case 2:
                        TempNode = TempNode.SelectSingleNode("CommParams");
                        CurNote = TempNode.SelectSingleNode("WarningLevel");
                        CurNote.InnerText = Config.WarningLevel.ToString();
                        CurNote = TempNode.SelectSingleNode("WarnInterval");
                        CurNote.InnerText = Config.WarnInterval.ToString();
                        CurNote = TempNode.SelectSingleNode("WarnEnable");
                        CurNote.InnerText = Config.WarnEnable ? "1" : "0";
                        CurNote = TempNode.SelectSingleNode("BaseLevel");
                        CurNote.InnerText = Config.BaseLevel.ToString();
                        CurNote = TempNode.SelectSingleNode("IncreaseLevel");
                        CurNote.InnerText = Config.IncreaseLevel.ToString();
                        CurNote = TempNode.SelectSingleNode("InterTimeSet");
                        CurNote.InnerText = Config.InterTimeSet.ToString();
                        break;
                    case 3:
                        TempNode = TempNode.SelectSingleNode("DTUParams");
                        CurNote = TempNode.SelectSingleNode("Authcode");
                        CurNote.InnerText = Config.Authcode.ToString();
                        CurNote = TempNode.SelectSingleNode("TelCount");
                        CurNote.InnerText = Config.TelCount.ToString();
                        CurNote = TempNode.SelectSingleNode("TelNum1");
                        CurNote.InnerText = Config.TelNum1.ToString();
                        CurNote = TempNode.SelectSingleNode("TelNum2");
                        CurNote.InnerText = Config.TelNum2.ToString();
                        CurNote = TempNode.SelectSingleNode("TelNum3");
                        CurNote.InnerText = Config.TelNum3.ToString();
                        CurNote = TempNode.SelectSingleNode("TelNum4");
                        CurNote.InnerText = Config.TelNum4.ToString();
                        CurNote = TempNode.SelectSingleNode("TelNum5");
                        CurNote.InnerText = Config.TelNum5.ToString();
                        CurNote = TempNode.SelectSingleNode("IpAddr1");
                        CurNote.InnerText = Config.IpAddr1.ToString();
                        CurNote = TempNode.SelectSingleNode("IpAddr2");
                        CurNote.InnerText = Config.IpAddr2.ToString();
                        CurNote = TempNode.SelectSingleNode("IpAddr3");
                        CurNote.InnerText = Config.IpAddr3.ToString();
                        CurNote = TempNode.SelectSingleNode("IpAddr4");
                        CurNote.InnerText = Config.IpAddr4.ToString();
                        CurNote = TempNode.SelectSingleNode("IpAddr5");
                        CurNote.InnerText = Config.IpAddr5.ToString();
                        CurNote = TempNode.SelectSingleNode("IpPort1");
                        CurNote.InnerText = Config.IpPort1.ToString();
                        CurNote = TempNode.SelectSingleNode("IpPort2");
                        CurNote.InnerText = Config.IpPort2.ToString();
                        CurNote = TempNode.SelectSingleNode("IpPort3");
                        CurNote.InnerText = Config.IpPort3.ToString();
                        CurNote = TempNode.SelectSingleNode("IpPort4");
                        CurNote.InnerText = Config.IpPort4.ToString();
                        CurNote = TempNode.SelectSingleNode("IpPort5");
                        CurNote.InnerText = Config.IpPort5.ToString();

                        break;
                }
                
            }
            catch 
            {
                R = false;
            }
            if (R == true) XmlDoc.Save(FilePath);
            return R;
        }
        #endregion 

        #region 读取配置文件
        //=======方法:读取整个XML文档内容===================================================
        public bool ReadConfig(string Path,int ConfigType)
        {
            FilePath = Path;
            if (File.Exists(FilePath) == false) return false;
            XmlDoc = new XmlDocument();                                                         //初始化XML对象
            XmlDoc.Load(FilePath);									                            //加载XML文件
            if (XmlDoc.HasChildNodes == false) return false;    		                            //是否XML合法或空文件
            try
            {
                XmlNode TempNode = XmlDoc.FirstChild;
                TempNode = TempNode.NextSibling;
                Xml_ReadContent(TempNode,ConfigType);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    
        
    }















}