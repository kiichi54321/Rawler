using RawlerLib.MyExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    public class GenerateUrl : RawlerBase, IInputParameter
    {
        List<KeyValue> parameterList = new List<KeyValue>();
        public RawlerCollection BeforeTrees { get; set; } = new RawlerCollection();
        public void AddParameter(string key, string value)
        {
            parameterList.Add(new KeyValue(key, value));
        }

        public void ReplaceParameter(string key, string value)
        {
            var tmp = parameterList.Where(n => n.Key == key);
            if(tmp.Any())
            {
                tmp.First().Value = value;
            }
            else
            {
                parameterList.Add(new KeyValue(key, value));
            }
        }

        public string Url { get; set; }

        public override void Run(bool runChildren)
        {
            parameterList.Clear();          
            if (BeforeTrees.Any())
            {
                foreach (var item in BeforeTrees)
                {
                    RawlerBase.GetText(GetText(), item, this);
                }
            }

            string url;
            if(string.IsNullOrEmpty(Url))
            {
                url = GetText();
            }
            else
            {
                url = Url;
            }

            url = url + "?" + parameterList.Select(n => $"{Uri.EscapeUriString( n.Key)}={Uri.EscapeUriString(n.Value)}").JoinText("&");

            SetText(url);
            base.Run(runChildren);  
        }

    }
}
