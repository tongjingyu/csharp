using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
namespace 更新程序
{
    class DownLoad
    {
        public static string HttpDownloadFile(string url, string path)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();

            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);

            byte[] bArr = new byte[10000];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();
            return path;
        }
        public static void DownLoadThread()
        {
            try
            {
                string Host = "http://www.trtos.com/update/";
                string FilePath = "";
              //  if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
                HttpDownloadFile(Host + "UpList.txt", FilePath + "UpList.txt");
                string[] str = File.ReadAllLines(FilePath + "UpList.txt", System.Text.Encoding.GetEncoding("gb2312"));
                for (int i = 0; i < str.Length; i++)
                {
                    int Offset = str[i].IndexOf('#');
                    if (Offset < 0) Offset = str[i].Length;
                    string FileName = str[i].Substring(0, Offset);
                   string  SaveName = FileName.Replace(".jpg", "");
                   HttpDownloadFile(Host + FileName, FilePath + SaveName);
                   Console.WriteLine("GetFile:" + SaveName);
                }
            }
            catch (Exception E) { Console.WriteLine(E.Message); }
        }
    }
}
