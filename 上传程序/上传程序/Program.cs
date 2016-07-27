using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
namespace 上传程序
{
    class Program
    {
        static void _Main(string[] args)
        {
            try
            {
                Console.WriteLine(args[0]);
                FtpHelper fh = new FtpHelper();
                File.Delete("temp.temp");
                File.Copy(args[1], "temp.temp");
                fh.Upload("tongjinlv", "071E34B236de0c", "temp.temp", "ftp://45.116.144.5/web/update/"+args[0]+"/"+ args[1] + ".jpg");
                File.Delete("temp.temp");
            }
            catch { }
        }
        static void Main(string[] args)//路径  /上传文件夹  /名称
        {
            try
            {
                Console.WriteLine("link to "+ "ftp://www.trtos.com/web/update/" + args[1] + "/" + args[2]);
                FtpHelper fh = new FtpHelper();
                File.Delete(args[0]+".temp");
                File.Copy(args[0], args[0] + ".temp");
                Console.WriteLine("update");
                fh.Upload("tongjinlv", "071E34B236de0c", args[0] + ".temp", "ftp://www.trtos.com/web/update/"+args[1]+"/" + args[2]);
                File.Delete(args[0] + ".temp");
                Console.WriteLine("succeed");
            }
            catch(Exception E) { Console.WriteLine(E.Message); }
        }
        static void __Main(string[] args)//路径  /上传文件夹  /名称
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\";
                Console.WriteLine(path);
                FtpHelper fh = new FtpHelper();
                File.Delete(path + "Project.bin.temp");
                File.Copy(path + "Project.bin", path + "Project.bin.temp");
                fh.Upload("tongjinlv", "071E34B236de0c", path + "Project.bin.temp", "ftp://45.116.144.5/web/update/STM32Bin/" + args[0]);
                File.Delete(path + "Project.bin.temp");
            }
            catch { }
        }
    }
}
