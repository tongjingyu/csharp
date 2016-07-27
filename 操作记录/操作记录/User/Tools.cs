using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Management;
namespace 透明时钟演示
{
    class Tools
    {
        public static string GetLocalHost()
        {
            IPAddress IPAddr = IPAddress.Parse("127.0.0.1");
            string IpList = "";
            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in arrIPAddresses)
            {
                if(ip.ToString().Length<16)IpList += ip.ToString()+";";
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    IPAddr = ip;
                }

            }
            return IpList;
        }
        public static string GetPublicIP()
        {
            Uri uri = new Uri("http://city.ip138.com/ip2city.asp");
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            req.Method = "get";
            using (Stream s = req.GetResponse().GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    char[] ch = { '[', ']' };
                    string str = reader.ReadToEnd();
                    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(str, @"\[(?<IP>[0-9\.]*)\]");
                    return m.Value.Trim(ch);

                }
            }
        }
        public static string GetHostName()
        {
            return "";
        }
        public static string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址 
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
        }
        public static string GetDiskID()
        {
            try
            {
                //获取硬盘ID 
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        public static string GetUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["UserName"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        public static string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["SystemType"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        public static string GetTotalPhysicalMemory()
        {
            try
            {
                //物理内存
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["TotalPhysicalMemory"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }
        public static string GetComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        public static void Test()
        {
            //MessageBox.Show(GetComputerName());
           // KeyBordHook.SetWindowsHookEx(1);
        }
        public static string GetUserInfor()
        {
            string Infor = "";
            Infor += "发送时间:" + DateTime.Now.ToString();    
        
            Infor += "<br>外网IP:";
            //Infor += GetIpInfor.getOutMessage();
            Infor += "<br>计算机名";
            Infor += GetComputerName();
            Infor += "<br>Mac地址:";
            Infor += GetMacAddress();
            Infor += "<br>硬盘ID:";
            Infor += GetDiskID();
            Infor += "<br>系统类型:";
            Infor += GetSystemType();
            Infor += "<br>物理内存";
            Infor += GetTotalPhysicalMemory();
            Infor += "<br>本机网卡IP:";
            Infor += GetLocalHost();
            return Infor;
        }
        public static string AddHead(object Data)
        {
            try
            {
                if (Data.GetType() == typeof(String)) return "'" + (string)Data + "'";
                else return Data.ToString();
            }
            catch { return ""; }
        }
        public static void GetInfor(ref PC_Infor PI)
        {
            PI.MAC = GetMacAddress();
            PI.PCName = GetComputerName();
            PI.DiskType = GetDiskID();
            PI.MemorySize = GetTotalPhysicalMemory();
            PI.UserName = GetUserName();
            PI.SysType = GetSystemType();
            PI.InIp = GetLocalHost();
        }
        public static void JoinRecord(string FileName, ref PC_Infor PI)
        {
                FileStream fs = new FileStream(FileName, FileMode.Open);

                StreamReader m_streamReader = new StreamReader(fs);

                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);// 从数据流中读取每一行，直到文件的最后一行

                string strLine = m_streamReader.ReadToEnd();
                m_streamReader.Close();
                PI.Record = strLine;
        }


    }
}
