using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Server
{
    class FileIO
    {
        private string BinPath;
        private string TextPath;
        private string BinCFGPath;
        public FileIO(string Path)
        {
            this.BinPath = Path + "\\Dev\\Upgrade\\";
            this.TextPath = Path + "\\Dev\\Cfg\\";
            this.BinCFGPath = Path + "\\Dev\\BinCfg\\";
            if (!Directory.Exists(this.BinPath)) Directory.CreateDirectory(this.BinPath);
            if (!Directory.Exists(this.TextPath)) Directory.CreateDirectory(this.TextPath); 
            if (!Directory.Exists(this.BinCFGPath)) Directory.CreateDirectory(this.BinCFGPath); 
        }
        public byte[] ReadBin(int ID)
        {
            string FileName = this.BinPath + ID + ".bin";
            FileInfo fi = new FileInfo(FileName);
            uint len = (uint)fi.Length;
            FileStream fs = new FileStream(FileName, FileMode.Open);
            byte[] fileBuf = new byte[len];
            fs.Read(fileBuf, 0, (int)len); 
            fs.Close();
            fs.Dispose();
            Value.WriteLog.WriteLine("读取文件:" + FileName, LogType.LT_Infor);
            return fileBuf;
        }
        public string[] ReadText(int ID)
        {
            string FileName = this.TextPath + ID + ".tpcfg";
            StreamReader sr = new StreamReader(FileName);
            string content = sr.ReadToEnd();
            sr.Close();
            Value.WriteLog.WriteLine("读取文件:" + FileName, LogType.LT_Infor);
            sr.Dispose();
            string[] str = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            return str;
        }
        public bool WriteBinCFG(int ID,byte[] Bin)
        {
            string FileName = this.BinCFGPath + ID + ".bin";
            FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(Bin);
            fs.Close();
            fs.Dispose();
            Value.WriteLog.WriteLine("写入文件:" + FileName, LogType.LT_Infor);
            return true;
        }
        public byte[] ReadBinCFG(int ID)
        {
            string FileName = this.BinCFGPath + ID + ".bin";
            FileInfo fi = new FileInfo(FileName);
            uint len = (uint)fi.Length;
            FileStream fs = new FileStream(FileName, FileMode.Open);
            byte[] fileBuf = new byte[len];
            fs.Read(fileBuf, 0, (int)len);
            fs.Close();
            fs.Dispose();
            Value.WriteLog.WriteLine("读取文件:" + FileName, LogType.LT_Infor);
            return fileBuf;
        }
    }
}
