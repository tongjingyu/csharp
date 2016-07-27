using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Configs
    {
        private string Path;
        private Ini ini;
        public Configs(string Path)
        {
            this.Path=Path;
            this.ini = new Ini(Path);
            
        }
        public void Load()
        {
                if(int.TryParse(ini.Read("ServerPort"),out Value.ServerPort)==false)ini.Write("ServerPort","9112");
        }
        public void Save()
        {
                ini.Write("ServerPort",Value.ServerPort.ToString());
        }
    }
}
