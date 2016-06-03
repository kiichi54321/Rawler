using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Rawler.Tool
{
    public class HttpClient:WebClient
    {
        HttpClientHandler hander = new System.Net.Http.HttpClientHandler();

        public override string HttpGet(string url, Encoding enc)
        {
            this.Sleep();
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(hander, false);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
            var result = client.GetStringAsync(url);
            result.Wait();
            return result.Result;
        }

        public override byte[] HttpGetByte(string url)
        {
            this.Sleep();
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(hander, false);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);

            var result = client.GetByteArrayAsync(url);
            result.Wait();
            
            return result.Result;
        }



        public override string HttpPost(string url, List<KeyValue> vals)
        {
            this.Sleep();
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(hander, false);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);
            var r = client.PostAsync(url, new FormUrlEncodedContent(vals.Select(n => new KeyValuePair<string, string>(n.Key, n.Value))));
            r.Wait();
            var r2 = r.Result.Content.ReadAsStringAsync();
            r2.Wait();
            return r2.Result;
        }

    }
}
