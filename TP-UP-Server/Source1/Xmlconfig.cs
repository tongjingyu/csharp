using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XmlConfiguration;
using System.Configuration;
namespace RTUService
{
    class XML
    {
        public static string XmlPath=Configs.Path+ "Configs.xml";
        static XmlDocument XmlDoc = new XmlDocument();
        public const string  RootStr="Configs";
        public const string  NodSqlStr="NodSqlConfig";
        public const string NodUserStr = "NodUserConfig";
        public const string  NodServiceStr = "NodServiceStr";
        public const string NodUserLogin = "NodUserLogin";


        public static void CreateXmlDoc()
        {
           
            XmlTextWriter Writer = new XmlTextWriter(XmlPath, null);
            //写入根元素
            Writer.WriteStartDocument();
            Writer.Formatting = Formatting.Indented;
             

            Writer.WriteStartElement(RootStr);

            /*******************************************/
            Writer.WriteStartElement(NodSqlStr);
            Writer.WriteElementString("SqlName", Configs.SqlName);
            Writer.WriteElementString("SqlPassWord", Configs.SqlPassWord);
            Writer.WriteElementString("SqlDataSource",Configs.SqlDataSource);
            Writer.WriteElementString("SqlDataBase", Configs.SqlDataBase);
            Writer.WriteElementString("SqlDensitySheet", Configs.SqlDensitySheet);
            Writer.WriteElementString("SqlRealTimeSheet", Configs.SqlRealTimeSheet);
            Writer.WriteElementString("SqlStationInfor", Configs.SqlStationInforSheet);
            Writer.WriteElementString("SqlConnectTimeOut", Configs.SqlConnectTimeOut.ToString());
            Writer.WriteElementString("RealSheetSize", Configs.RealSheetSize.ToString());
            Writer.WriteElementString("DensitySheetSize", Configs.DensitySheetSize.ToString());

            Writer.WriteElementString("OracleDataBase", Configs.OracleDataBase);
            Writer.WriteElementString("OracleDataSource", Configs.OracleDataSource);
            Writer.WriteElementString("OracleName", Configs.OracleName);
            Writer.WriteElementString("OraclePassword", Configs.OraclePassword);
            Writer.WriteElementString("OraclePoint", Configs.OraclePoint.ToString());
            Writer.WriteEndElement();
            /******************************************/
            Writer.WriteStartElement(NodUserLogin);
            Writer.WriteElementString(Configs.UserRootName, Configs.UserRootPassWord);
            Writer.WriteEndElement();

            /******************************************/            
            Writer.WriteStartElement(NodServiceStr);
            Writer.WriteElementString("ServicePoint", Configs.ServicePoint.ToString());
            Writer.WriteElementString("AgreementAAASize", Configs.AgreementAAASize.ToString());
            Writer.WriteElementString("AgreementAAAHead", Configs.AgreementAAAHead);
            Writer.WriteElementString("AgreementBAAHead", Configs.AgreementBAAHead.ToString());
            Writer.WriteElementString("ReviceTimeOut", Configs.ReviceTimeOut.ToString());
            Writer.WriteElementString("AgainFailCount", Configs.AgainFailCount.ToString());
            Writer.WriteEndElement();
            /******************************************/

            /******************************************/
            Writer.WriteStartElement(NodUserStr);
            Writer.WriteElementString("UserFacePoint", Configs.UserFacePoint.ToString());
            Writer.WriteElementString("UserTimeOut", Configs.UserTimeOut.ToString());
            Writer.WriteEndElement();
            /******************************************/

            Writer.WriteEndElement();
            //将XML写入文件并且关闭XmlTextWriter
            Writer.Close();
        }
        public static bool LoadConfig()
        {
            bool Re = true;
            try
            {
                XmlDoc.Load(XmlPath);
                Re = true;
            }
            catch
            {
                Re = false;
                CreateXmlDoc();//不存在配置文件时自动成该默认配置文件
            }
            string NodString = NodSqlStr;
            ReSetConfigS(ref Configs.SqlName,NodString, "SqlName");
            ReSetConfigS(ref Configs.SqlPassWord, NodString, "SqlPassWord");
            ReSetConfigS(ref Configs.SqlDataSource, NodString, "SqlDataSource");
            ReSetConfigS(ref Configs.SqlDataBase, NodString, "SqlDataBase");
            ReSetConfigS(ref Configs.SqlStationInforSheet, NodString, "SqlStationInfor");
            ReSetConfigS(ref Configs.SqlRealTimeSheet, NodString, "SqlRealTimeSheet");
            ReSetConfigS(ref Configs.SqlDensitySheet, NodString, "SqlDensitySheet");
            ReSetConfigD(ref Configs.SqlConnectTimeOut, NodString, "SqlConnectTimeOut");
            ReSetConfigD(ref Configs.RealSheetSize, NodString, "RealSheetSize");
            ReSetConfigD(ref Configs.DensitySheetSize, NodString, "DensitySheetSize");

            ReSetConfigS(ref Configs.OracleDataBase, NodString, "OracleDataBase");
            ReSetConfigS(ref Configs.OracleDataSource, NodString, "OracleDataSource");
            ReSetConfigS(ref Configs.OracleName, NodString, "OracleName");
            ReSetConfigS(ref Configs.OraclePassword, NodString, "OraclePassword");
            ReSetConfigD(ref Configs.OraclePoint, NodString, "OraclePoint");

            NodString = NodServiceStr;
            ReSetConfigD(ref Configs.ReviceTimeOut, NodString, "ReviceTimeOut");
            ReSetConfigS(ref Configs.AgreementAAAHead,NodString, "AgreementAAAHead");
            ReSetConfigD(ref Configs.AgreementAAASize, NodString, "AgreementAAASize");
            ReSetConfigS(ref Configs.AgreementBAAHead, NodString, "AgreementBAAHead");
            ReSetConfigD(ref Configs.ServicePoint, NodString, "ServicePoint");
            ReSetConfigD(ref Configs.AgainFailCount, NodString, "AgainFailCount");
            NodString = NodUserStr;
            ReSetConfigD(ref Configs.UserFacePoint, NodString, "UserFacePoint");
            ReSetConfigD(ref Configs.UserTimeOut, NodString, "UserTimeOut");
            return Re;
        }
        //设置单个节点
        public static bool SetConfigD(ref int D, string S)
        {
            if (S== "") return false;
            D =int.Parse(S.Trim());
            CreateXmlDoc();
            return true;
        }
        public static bool SetConfigS(ref string D, string S)
        {
            string Str = D.ToString();
            if (Str == "") return false;
            D = S;
            CreateXmlDoc();
            return true;
        }
        public static bool ReSetConfigS(ref string D,string Nod, string S)
        {
            string SS = S;
            try
            {
                S = ReadVal(RootStr, Nod, S);
            }
            catch(Exception E)
            {
                CreateInfor.WriteLogs(E.Message+"没有找到对象:"+S);
            }
            if (S == "") { CreateInfor.WriteLogs("配置项为空，已加载默认值，请检查！" + SS); CreateXmlDoc(); return false; }
            D = S;
            return true;
        }
        public static bool ReSetConfigD(ref int D,string Nod, string S)
        {
            string SS = S;
            try
            {
                S = ReadVal(RootStr, Nod, S);
            }
            catch (Exception E)
            {
                CreateInfor.WriteLogs(E.Message+"没有找到对象:"+ S);
            }
            if (S == "") { CreateInfor.WriteLogs("配置项为空，已加载默认值，请检查！" + SS); CreateXmlDoc(); return false; }
            try
            {
                D = int.Parse(S);
            }
            catch { }
            return true;
        }
        public static bool ResetVal(string Section,string Nod, string Key, string Val)
        {
            XmlNodeList nodeList = XmlDoc.SelectSingleNode(Section).ChildNodes;
            foreach (XmlNode xn in nodeList)//遍历所有子节点
            {
                XmlNodeList nls = xn.ChildNodes;//继续获取xe子节点的所有子节点
                if (xn.Name == Nod)
                {
                    foreach (XmlNode xn1 in nls)//遍历
                    {
                        XmlElement xe2 = (XmlElement)xn1;//转换类型
                        if (xe2.Name == Key)//如果找到
                        {
                            xe2.InnerText = Val;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static string  ReadVal(string Section,string Nod, string Key)
        {
            XmlNodeList nodeList = XmlDoc.SelectSingleNode(Section).ChildNodes;
            foreach (XmlNode xn in nodeList)//遍历所有子节点
            {
                XmlElement xe = (XmlElement)xn;
                if (xn.Name == Nod)
                {
                    XmlNodeList nls = xn.ChildNodes;//继续获取xe子节点的所有子节点
                    foreach (XmlNode xn1 in nls)//遍历
                    {
                        XmlElement xe2 = (XmlElement)xn1;//转换类型
                        if (xe2.Name == Key)//如果找到
                        {
                            return (xe2.InnerText);
                        }
                    }
                }
            }
            return "";
        }
        public static bool ReadLogin(string Name, string PassWord)
        {
            LoadConfig();
            XmlNodeList nodeList = XmlDoc.SelectSingleNode(RootStr).ChildNodes;
            foreach (XmlNode xn in nodeList)//遍历所有子节点
            {
                XmlElement xe = (XmlElement)xn;
                if (xn.Name == NodUserLogin)
                {
                    XmlNodeList nls = xn.ChildNodes;//继续获取xe子节点的所有子节点
                    foreach (XmlNode xn1 in nls)//遍历
                    {
                        XmlElement xe2 = (XmlElement)xn1;//转换类型
                        if (xe2.Name == Name)//如果找到
                        {
                            if (xe2.InnerText == PassWord) return true;
                            else return false;
                        }
                    }
                }
            }
            return false;
        }
        public static string ReadNotCreate(string Section, string Nod, string Key, string DefVal)
        {
            string temp = ReadVal(Section, Nod, Key);
            if (temp == "")
            {
                ResetVal(Section, Nod, Key, DefVal);
                return DefVal;
            }
            else return temp;
        }
        public void InsertNodeWithChild(string mainNode, string ChildNode, string Element, string Content)
        {
            XmlNodeList nodeList = XmlDoc.SelectSingleNode(mainNode).ChildNodes;
            foreach (XmlNode xn in nodeList)//遍历所有子节点
            {
                XmlNodeList nls = xn.ChildNodes;//继续获取xe子节点的所有子节点
                if (xn.Name == ChildNode)
                {
                    XmlElement elem = XmlDoc.CreateElement("price");
                    elem.InnerText = "19.95";

                    //Add the node to the document.
                    XmlDoc.InsertAfter(elem, xn.FirstChild);

                }
            }
        }
        public void DelectNodeWithChild(string mainNode, string ChildNode, string Element)
        {

        }
    }
}