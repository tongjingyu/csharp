using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Windows.Forms;
namespace SensorFace
{
    class DownLoad
    {
        public static string HttpDownloadFile(string url, string path)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();

            //创建本地文件写入流
            Stream stream = new FileStream(path, FileMode.Create);

            byte[] bArr = new byte[1024];
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
            int z = 4;
            while(z-->0)
            {
                R:
            try { 
             string Host= "http://www.trtos.com/update/STM32Bin/";
             string FilePath = "File/";
             if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
             HttpDownloadFile(Host + "List.txt", FilePath + "List.txt");
             string[] str = File.ReadAllLines(FilePath + "List.txt", System.Text.Encoding.GetEncoding("gb2312"));
            for(int i=0;i<str.Length;i++)
            {
                int Offset=str[i].IndexOf('#');
                if(Offset<0)Offset=str[i].Length;
                string FileName = str[i].Substring(0, Offset);
                string SaveName = FileName.Replace(".bin", ".bin.tmp");
                HttpDownloadFile(Host + FileName, FilePath + SaveName);
                File.Delete(FilePath + FileName);
                File.Move(FilePath + SaveName, FilePath + FileName);
            }
            }
            catch{ goto R; }
                Value.UpdateBin = true;
                return;
            }
            MessageBox.Show("更新失败");
        }
    }
}
