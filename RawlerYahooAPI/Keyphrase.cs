using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using System.Xml.Linq;

namespace RawlerYahooAPI
{
    public class Keyphrase : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Keyphrase>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public string ApiId { get; set; }


        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            string apiId = ApiId;
            if (string.IsNullOrEmpty(ApiId))
            {
                apiId = Rawler.Tool.TempVar.GetVar("YahooApiId");
            }
            if (string.IsNullOrEmpty(apiId))
            {
                Rawler.Tool.ReportManage.ErrReport(this, "YahooApiIdがありません。SetTmpValで指定してください");
                return;
            }
   
            string baseUrl = "http://jlp.yahooapis.jp/KeyphraseService/V1/extract";
            var post = "appid=" + apiId + "&sentence=" +Uri.EscapeUriString(GetText());
            string result = string.Empty;
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.Headers.Add("Content-Type","application/x-www-form-urlencoded");
                wc.Encoding = Encoding.UTF8;
                result = wc.UploadString(new Uri(baseUrl), "POST", post);
                wc.Dispose();
            }
            catch(Exception e)
            {
                ReportManage.ErrReport(this, e.Message +"\t"+GetText());
            }
            if (result != string.Empty)
            {
                var root = XElement.Parse(result);
                var ns = root.GetDefaultNamespace();
                var list = root.Descendants(ns + "Result").Select(n => new KeyphraseResult() { Keyphrase = n.Element(ns + "Keyphrase").Value, Score = double.Parse(n.Element(ns + "Score").Value) });

                List<string> list2 = new List<string>();
                foreach (var item in list)
                {
                    list2.Add(Codeplex.Data.DynamicJson.Serialize(item));
                }

                base.RunChildrenForArray(runChildren, list2);
            }
        }

        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }



        public class KeyphraseResult
        {
            public string Keyphrase { get; set; }
            public double Score { get; set; }
        }

        public class KeyphraseResults
        {
            public List<KeyphraseResult> Results { get; set; }
            public object Tag { get; set; }
        }
    }
}
