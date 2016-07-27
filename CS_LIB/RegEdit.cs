using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
namespace 奇奇邮件助手
{
    class RegEdit
    {
       
        public static string GetReg(string name)
        {
            try
            {
                string registData;
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
                RegistryKey aimdir = software.OpenSubKey("qiqimail", true);
                registData = aimdir.GetValue(name).ToString();
                return registData;
            }
            catch { return ""; }
        }
        public static void SetReg(string name, string tovalue)
        {
            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey software = hklm.OpenSubKey("SOFTWARE", true);
            RegistryKey aimdir = software.CreateSubKey("qiqimail");
            aimdir.SetValue(name, tovalue);
        } 
    }
    
}
