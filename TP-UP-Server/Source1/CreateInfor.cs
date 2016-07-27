using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace RTUService
{
    class CreateInfor
    {
        public static void WriteLogs(string Text)
        {
            try
            {
                string NowName = Configs.Path + "NewLogs.txt";
                FileInfo Finfo = new FileInfo(NowName);
                lock (Finfo)
                {
                    if (Finfo.Exists && Finfo.Length > 1024 * 100)
                    {
                        for (int i = 0; i < 1000; )
                        {
                            string OldName = Configs.Path + "OldLogs[" + i.ToString() + "].txt";
                            if (File.Exists(OldName)) i++;
                            else
                            {
                                File.Move(NowName, OldName);
                                break;
                            }
                        }
                    }
                    StreamWriter WT = new StreamWriter(NowName, true);
                    string Msg = "[" + DateTime.Now.ToString() + "]:" + Text;
                    WT.WriteLine(Msg);
                    Console.WriteLine(Msg);
                    WT.Close();
                }
            }
            catch { SysFlag.ServiceStart = false; }
        }
    }
}
