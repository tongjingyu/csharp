using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Html
    {
        public static string AddTextBoxLable(string Name,string Text,string Lable,int FontSize)
        {
            if (Text.Length>0) Text = "value=" + Text;
            string Str = "<P align=left >" +
                "<INPUT style=\"FONT-SIZE: "+FontSize+"pt; WIDTH: 500px\" size=5 "+Text+" name=" + Name + ">" +
                "<STRONG><FONT size=7 color=#008000 ><SPAN id=wiz_font_size_span_285640406 style=\"FONT-SIZE: "+FontSize+"pt\">"+Lable+"\r\n</FONT></STRONG>" +
                "</P>";
            return Str;
        }
        public static string AddHead(bool online)
        {
            string Title, Color;
            if(online)
            {
                Title = "设备状态：在线\r\n";
                Color = "#008000";
            }else
            {
                Title = "设备状态：离线\r\n";
                Color = "#80000";
            }
            string msg="HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nPragma: no-cache\r\n\r\n" +
           "<title>TP配置</title>" +
            "<style> " +
           ".divcss5-cent{margin:0 auto;width:800px;height:auto;border:0px solid #bbb} " +
           "</style> " +
           "<html>" +
           "<head>" +
           "<body style=\"background-color:#eee\">" +
           "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\">" +
           "<P align=center><STRONG><FONT size=7 color=" + Color + "><SPAN id=wiz_font_size_span_285640406 style=\"FONT-SIZE: 40pt\">" +
           Title+
           "</FONT></STRONG></P>" +
           "<HR color=#ff0000>" +
           "<form  method=\"post\">" +
            "<div class=\"divcss5-cent\">";
            return msg;
        }
        public static string AddEnd()
        {
            string msg=
            "</div> " +
            "</form>" +
            "</html>";
            return msg;
        }
        public static string AddrTwoButton(string name1,string name2,int FontSize)
        {
            string Str =
                 "<P align=left >" +
                  "<INPUT style=\"FONT-SIZE: " + FontSize + "pt;WIDTH: 300px\"  type=submit value=检查 name=ButtonSubmit1>" +
                  "<INPUT style=\"float:right;  FONT-SIZE: "+FontSize+"pt;WIDTH: 300px\"  type=submit value=提交 name=ButtonSubmit2>" +
                  "</P>";
            return Str;
        }
        public static string GetHttpValue(string msg)
        {
            try
            {
                string n = msg.Substring(msg.IndexOf('?') + 1, 8);
                return n;
            }
            catch { return ""; }
        }
    }
}
