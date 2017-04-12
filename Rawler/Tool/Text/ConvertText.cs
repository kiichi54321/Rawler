using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    /// <summary>
    /// 辞書に基づいて変換する
    /// </summary>
    public class ConvertText:RawlerBase
    {
        Dictionary<string, string> convertDic = new Dictionary<string, string>();
        string dic = null;

        /// <summary>
        /// "item1:アイテム1,item2:アイテム2　というようなテキストを辞書に変換する。
        /// </summary>
        public string Dic { get; set; }

        string tmp_dic_text;
        void CreateDictionary(string input)
        {
            var tmp = input.Convert(this);
            if (tmp_dic_text != tmp && tmp != null)
            {
                convertDic = tmp.Split(',').Where(n=>n.Length>0).ToDictionary(n => n.Split(':').First().Trim(), n => n.Split(':').Last().Trim());
                tmp_dic_text = tmp;
            }
        }

        public string NotFoundText { get; set; }

        public override void Run(bool runChildren)
        {
            string text = GetText();
            CreateDictionary(Dic);
            if(convertDic.ContainsKey(text))
            {
                SetText(convertDic[text]);
            }
            else
            {
                if(string.IsNullOrEmpty( NotFoundText))
                {
                    SetText(text);
                }
                else
                {
                    SetText(NotFoundText);
                }
            }

            base.Run(runChildren);  
        }
    }
}
