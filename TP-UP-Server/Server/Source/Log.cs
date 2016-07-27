using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Server
{
    class Log
    {

        private String FilePath;
        private LogType LT;
        public Log(String FilePath,LogType LT)
        {
            this.FilePath = FilePath + "\\Log\\";
            if (!Directory.Exists(this.FilePath)) Directory.CreateDirectory(this.FilePath);
            this.LT = LT;
        }
        public void WriteLine(String msg,LogType LT)
        {
            string Msg = "[" + DateTime.Now.ToString() + "]:" + msg;
            if (LT < this.LT)
            {
                if (TagetValue.SystemType == SoftType.ST_Console) Console.WriteLine(Msg);
                if (TagetValue.SystemType == SoftType.ST_Server)
                {

                    string NowName = FilePath + "NewLogs.txt";
                    FileInfo Finfo = new FileInfo(NowName);
                    lock (Finfo)
                    {
                        if (Finfo.Exists && Finfo.Length > 1024 * 100)
                        {
                            for (int i = 0; i < 1000; )
                            {
                                string OldName = FilePath + "OldLogs[" + i.ToString() + "].txt";
                                if (File.Exists(OldName)) i++;
                                else
                                {
                                    File.Move(NowName, OldName);
                                    break;
                                }
                            }
                        }
                        StreamWriter WT = new StreamWriter(NowName, true);
                        WT.WriteLine(Msg);
                        WT.Close();
                    }
                };
            }
        }
    }
}
