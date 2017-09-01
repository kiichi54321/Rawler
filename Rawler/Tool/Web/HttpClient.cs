using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Rawler.Tool
{
    /// <summary>
    /// System.Net.Http.HttpClient を使ったWebClient
    /// </summary>
    public class HttpClient : WebClient
    {
        HttpClientHandler hander = new System.Net.Http.HttpClientHandler();
        System.Net.Http.HttpClient client;

        public HttpHeaders HttpHeaders { get; set; }

        System.Net.Http.HttpClient GetHttpClient()
        {
            if (client == null)
            {
                client = new System.Net.Http.HttpClient(hander, false);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(this.UserAgent);

            }
            if (HttpHeaders != null)
            {
                client.SetHeader(HttpHeaders);
            }
            return client;
        }

        public override string HttpGet(string url, Encoding enc)
        {
            this.Sleep();
            if (ReportUrl)
            {
                ReportManage.Report(this, "GET " + url, true, true);
            }
            System.Net.Http.HttpClient client = GetHttpClient();
            var result = client.GetStringAsync(url);
            result.Wait();
            return result.Result;
        }




        public override byte[] HttpGetByte(string url)
        {
            this.Sleep();
            System.Net.Http.HttpClient client = GetHttpClient();
            if (!string.IsNullOrEmpty(this.Referer))
            {
                client.DefaultRequestHeaders.Referrer = new Uri(this.Referer);
            }
            var result = client.GetByteArrayAsync(url);
            result.Wait();

            return result.Result;
        }



        public override string HttpPost(string url, List<KeyValue> vals)
        {
            this.Sleep();
            System.Net.Http.HttpClient client = GetHttpClient();
            var r = client.PostAsync(url, new FormUrlEncodedContent(vals.Select(n => new KeyValuePair<string, string>(n.Key, n.Value))));
            r.Wait();
            var r2 = r.Result.Content.ReadAsStringAsync();
            r2.Wait();
            return r2.Result;
        }

        public string HttpGet(string url, List<KeyValue> parameterList, List<KeyValue> httpHeaderList)
        {
            this.Sleep();
            if (ReportUrl)
            {
                ReportManage.Report(this, "GET " + url, true, true);
            }
            System.Net.Http.HttpClient client = GetHttpClient();

            client.SetHeader(httpHeaderList);
            var result = client.GetStringAsync(url);
            result.Wait();
            client.RemoveHeader(httpHeaderList);
            return result.Result;
        }

        public string HttpPost(string url, List<KeyValue> parameterList, List<KeyValue> httpHeaderList)
        {
            this.Sleep();
            System.Net.Http.HttpClient client = GetHttpClient();
            client.SetHeader(httpHeaderList);
            if (ReportUrl)
            {
                ReportManage.Report(this, $"Post {url} { Newtonsoft.Json.JsonConvert.SerializeObject(parameterList.ToDictionary(n=>n.Key,n=>n.Value))}.", true, true);
            }

            var r = client.PostAsync(url, new FormUrlEncodedContent(parameterList.Select(n => new KeyValuePair<string, string>(n.Key, n.Value))));
            r.Wait();
            var r2 = r.Result.Content.ReadAsStringAsync();
            r2.Wait();
            return r2.Result;
        }
    }

    /// <summary>
    /// Httpのヘッダー
    /// </summary>
    public class HttpHeaders : List<KeyValue>
    {
    }


    public static class HttpClientExtend
    {
        public static void SetHeader(this System.Net.Http.HttpClient client, List<KeyValue> headers)
        {
            if (headers == null) return;
            foreach (var item in headers.GroupBy(n => n.Key))
            {
                client.DefaultRequestHeaders.Add(item.Key, item.Select(n => n.Value));
            }
        }

        public static void RemoveHeader(this System.Net.Http.HttpClient client, List<KeyValue> headers)
        {
            if (headers == null) return;
            foreach (var item in headers.GroupBy(n => n.Key))
            {
                client.DefaultRequestHeaders.Remove(item.Key);
            }
        }
    }
}
