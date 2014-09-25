using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// KeyとValueをセットにしたクラス
    /// </summary>
    [ContentProperty("Value")]    
    public class KeyValue
    {
        public KeyValue()
        {
        }
        public KeyValue(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ContentProperty("Value")]
    public class TextVaule
    {
        public TextVaule()
        {
        }
        public TextVaule(string txt)
        {
            this.Value = txt;
        }
        public string Value { get; set; }
    }
    [Serializable]
    public class TextVauleList : System.Collections.Generic.List<TextVaule>
    {
        public void Add(string text)
        {
            this.Add(new TextVaule(text));
        }

        public List<string> GetList()
        {
            List<string> list = new List<string>();
            foreach (var item in this)
            {
                list.Add(item.Value);
            }
            return list;
        }

        public void SetList(List<string> list)
        {
            foreach (var item in list)
            {
                this.Add(new TextVaule(item));
            }
        }
    }
}
