using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rawler.NPL
{
    /// <summary>
    /// Chopの基本クラス
    /// </summary>
    public static class Chop
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string ChopText(this string text, string pattern)
        {
            System.Text.RegularExpressions.Regex r = new Regex(pattern);
            List<string> list = new List<string>();
            foreach (System.Text.RegularExpressions.Match item in r.Matches(text))
            {
                list.Add(item.Value);
            }
            foreach (var item in list)
            {
                // List.Add(item);
                text = text.Replace(item, " ");
            }
            return text;
        }
    }


    public class ChopUrl:Rawler.Tool.RawlerBase
    {
        public override void Run(bool runChildren)
        {
            SetText(GetText().ChopText(@"https?://[\w/:%#\$&\?\(\)~\.=\+\-]+"));
            base.Run(runChildren);
        }
    }

   
}
