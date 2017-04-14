using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler.Tool
{
    public class Post : RawlerBase, IInputParameter,IHttpHeader
    {
        private List<KeyValue> parameterList = new List<KeyValue>();
        private List<KeyValue> httpHeaderList = new List<KeyValue>();


        public void AddParameter(string key, string value)
        {
            parameterList.Add(new KeyValue(key, value));
        }

        public void ReplaceParameter(string key, string value)
        {
            var list = parameterList.Where(n => n.Key == key);
            if(list.Any())
            {
                list.First().Value = value;
            }
            else
            {
                parameterList.Add(new KeyValue(key, value));
            }
        }

        public void AddHttpHeader(string key, string value)
        {
            httpHeaderList.Add(new KeyValue(key, value));
        }
       
        public string Url { get; set; }
        public RawlerCollection BeforeTrees { get; private set; } = new RawlerCollection();

        public override void Run(bool runChildren)
        {
            parameterList.Clear();
            httpHeaderList.Clear();

            BeforeTrees.Run(this, GetText());
            var client = (HttpClient)this.GetUpperInterface<HttpClient>();
            client.HttpPost(Url.Convert(this), parameterList, httpHeaderList);

            base.Run(runChildren);
        }

    }
}
