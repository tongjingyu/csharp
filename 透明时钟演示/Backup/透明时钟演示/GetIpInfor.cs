using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
namespace 透明时钟演示
{
   
    class GetIpInfor
    {

        public static string Content;
        public static void getOutMessage()
        {


            string strUrl = "http://iframe.ip138.com/ic.asp"; //获得IP的网址了

            Uri uri = new Uri(strUrl);
            WebRequest wr = WebRequest.Create(uri);
            Stream s = wr.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(s, Encoding.Default);
            Content = sr.ReadToEnd(); //读取网站的数据
        }
        /// <summary>
        /// 外网IP
        /// </summary>
        /// <returns>外网IP地址</returns>
        public static string getOutIp()
        {
            int i = Content.IndexOf("[") + 1;
            string ip = Content.Substring(i, 14);
            string ips = ip.Replace("]", "").Replace(" ", "");
            return ips;
        }
        /// <summary>
        /// 省份
        /// </summary>
        /// <returns>省份</returns>
        public static string getOutProvince()
        {
            int i = Content.IndexOf("自") + 2;
            string province = Content.Substring(i, Content.IndexOf("省") - i + 1);
            return province;
        }
        /// <summary>
        /// 城市
        /// </summary>
        /// <returns>城市</returns>
        public static string getOutCity()
        {
            int i = Content.IndexOf("省") + 1;
            string city = Content.Substring(i, Content.IndexOf("市") - i + 1);
            return city;
        }
        /// <summary>
        /// 运营商
        /// </summary>
        /// <returns>运营商</returns>
        public static string getOutProvider()
        {
            int i = Content.IndexOf("市") + 2;
            string provider = Content.Substring(i, 2);
            return provider;
        }
            public static void GetInfor(ref PC_Infor PI)
        {
            getOutMessage();
            PI.OutIp = getOutIp();
            PI.Provider = getOutProvider();
            PI.Province = getOutProvince();
            PI.City = getOutCity();
        }
    }
}
