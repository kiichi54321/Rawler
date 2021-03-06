﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTwitter
{
    public class TwitterSearch : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<TwitterSearch>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            ReadSearch();
            GC.Collect();
        }

        void Post(IEnumerable<string> list)
        {
            base.RunChildrenForArray(true, list);

        }

        public double SleepSecond { get; set; }

        private Dictionary<string, Tuple<decimal, decimal>> searchWordDic = new Dictionary<string, Tuple<decimal, decimal>>();
        

        public string Language { get; set; }
        public string GeoCode { get; set; }
        public string Locale { get; set; }

        public int MaxLoopCount { get; set; } = int.MaxValue;
        public string SearchWord { get; set; }

        protected void ReadSearch()
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
            bool flag = true;
     
            decimal maxId = -1;
            decimal minId = decimal.MaxValue;
            string searchWord = GetText();
            if (string.IsNullOrEmpty(SearchWord) == false)
            {
                searchWord = SearchWord.Convert(this);
            }
            bool isFrist = true;
            if (searchWordDic.ContainsKey(searchWord)) isFrist = false;

            int loop = 1;
            while (flag)
            {
                ReportManage.Report(this, loop+"回目", true, true);
                loop++;
                /// <para>- <c>string</c> q (required)</para>
                /// <para>- <c>string</c> geocode (optional)</para>
                /// <para>- <c>string</c> lang (optional)</para>
                /// <para>- <c>string</c> locale (optional)</para>
                /// <para>- <c>string</c> result_type (optional)</para>
                /// <para>- <c>int</c> count (optional)</para>
                /// <para>- <c>string</c> until (optional)</para>
                /// <para>- <c>long</c> since_id (optional)</para>
                /// <para>- <c>long</c> max_id (optional)</para>
                /// <para>- <c>bool</c> include_entities (optional)</para>
                
                Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                       {"q", searchWord},
                       {"geocode", this.GeoCode},
                       {"lang", this.Language},
                       {"locale", this.Locale},
                       {"result_type", "recent"},
                        {"count", 100}, 
                     //   {"until", },
                     //   {"max_id",}, 
                     //   {"include_entities", "paint.exe"},                      
                };

                if (minId != decimal.MaxValue) dic.Add("max_id",minId-1);


                if (isFrist == false && searchWordDic.ContainsKey(searchWord))
                {
                    dic.Add("since_id", searchWordDic[searchWord].Item1);
                }
                try
                {
                    var result = login.Token.Search.Tweets(dic.Where(n => n.Value != null));


                    int c = 0;
                    int d = 0;
                    if (result != null)
                    {
                        List<string> list = new List<string>();
                        //Jsonを下流に流す
                        foreach (var item in result)
                        {
                            maxId = Math.Max(maxId, item.Id);
                            minId = Math.Min(minId, item.Id);

                            if (searchWordDic.ContainsKey(searchWord))
                            {
                                if (searchWordDic[searchWord].Item1 < item.Id || searchWordDic[searchWord].Item2 > item.Id)
                                {
                                    c++;
                                    list.Add(Newtonsoft.Json.Linq.JObject.FromObject(item).ToString());
                                    //  yield return Codeplex.Data.DynamicJson.Serialize(item);
                                }
                                else
                                {
                                    d++;
                                }
                            }
                            else
                            {
                                c++;
                                list.Add(Newtonsoft.Json.Linq.JObject.FromObject(item).ToString());
                                // yield return Codeplex.Data.DynamicJson.Serialize(item);
                                //   yield return Codeplex.Data.DynamicJson.Serialize(item);
                            }
                        }
                        Post(list);

                        //すでに取得したものを含むとき
                        if (d > 0 || c == 0)
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    if (searchWordDic.ContainsKey(searchWord))
                    {
                        searchWordDic[searchWord] = new Tuple<decimal, decimal>(maxId, minId);
                    }
                    else
                    {
                        searchWordDic.Add(searchWord, new Tuple<decimal, decimal>(maxId, minId));
                    }
                
                }
                catch(Exception ex)
                {
                    ReportManage.ErrReport(this, ex.ToString());
                    if(ex.Message.Contains("Rate limit exceeded"))
                    {
                        ReportManage.Report(this, "30秒停止");
                        System.Threading.Thread.Sleep((int)(30 * 1000));
                    }
                    else
                    {
                        flag = false;
                    }
                }
                if (MaxLoopCount > loop) break;

                if (SleepSecond > 0)
                {
                    System.Threading.Thread.Sleep((int)(SleepSecond * 1000));
                }
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


    }


}
