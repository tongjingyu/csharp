using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Web;
namespace 更新程序
{
    class Program
    {
        public static bool IsFileOpen(string filePath)
        {
            bool result = false;
            System.IO.FileStream fs = null;
            try
            {
                fs = File.OpenWrite(filePath);
                fs.Close();
            }
            catch
            {
                result = true;
            }
            return result;
        }
        static void Main(string[] args)
        {
            try
            {
                string Host = "http://www.trtos.com/update/"+args[0]+"/";
              
                string FilePath = "";
                //  if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
                DownLoad.HttpDownloadFile(Host + "List.txt", FilePath + "UpList.txt");
                Console.WriteLine(Host);
                string[] str = File.ReadAllLines(FilePath + "UpList.txt", System.Text.Encoding.GetEncoding("gb2312"));
                for (int i = 0; i < str.Length; i++)
                {
                    int Offset = str[i].IndexOf('#');
                    if (Offset < 0) Offset = str[i].Length;
                    string FileName = str[i].Substring(0, Offset);
                    Console.WriteLine("GetFile:" + FileName);
                    DownLoad.HttpDownloadFile(Host + FileName, FilePath + FileName);
                   
                }
                for (int i = 0; i < str.Length; i++)
                {
                    int Offset = str[i].IndexOf('#');
                    if (Offset < 0) Offset = str[i].Length;
                    string FileName = str[i].Substring(0, Offset);
                    string SaveName = FileName.Replace(".jpg", "");
                    while (IsFileOpen(FilePath + SaveName)) Thread.Sleep(1000);
                    File.Delete(FilePath + SaveName);
                    Console.WriteLine("DeleteFile:" + SaveName);
                    System.IO.File.Move(FileName, FilePath+SaveName);

                }
                File.Delete("UpList.txt");
                Console.WriteLine("更新完毕");
            }
            catch (Exception E) { Console.WriteLine(E.Message); }
        }
    }
}
