using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.International.Converters.PinYinConverter;
namespace 查域名
{
    public static class ChineseCharacters
    {
        public static string GetPinyin(string Hz)
        {
            char[] hanzi = Hz.ToArray();
            String firstPinyin ="";
            foreach (char c in hanzi)
            {
                ChineseChar chineseChar = new ChineseChar(c);
                var pinyins = chineseChar.Pinyins;

                foreach (var pinyin in pinyins)
                {
                    if (pinyin != null)
                    {
                        firstPinyin += pinyin.Substring(0, pinyin.Length - 1);
                        break;
                    }
                }
            }
            return firstPinyin.ToLower();
        }
      
    }
}