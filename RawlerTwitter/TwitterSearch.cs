using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using Twitterizer;

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

            base.RunChildrenForArray(runChildren, ReadSearch());

        }

        private Dictionary<string, Tuple<decimal, decimal>> searchWordDic = new Dictionary<string, Tuple<decimal, decimal>>();
        

        public string Language { get; set; }
        public string GeoCode { get; set; }
  //      public decimal SinceId { get; set; }
        public DateTime UntilDate { get; set; }
        public string SearchWord { get; set; }
        SearchOptionsResultType resultType = SearchOptionsResultType.Recent;

        public SearchOptionsResultType ResultType
        {
            get { return resultType; }
            set { resultType = value; }
        }

        protected IEnumerable<string> ReadSearch()
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
            bool flag = true;
     
            decimal maxId = -1;
            decimal minId = decimal.MaxValue;
            string searchWord = GetText();
            if (string.IsNullOrEmpty(SearchWord) == false)
            {
                searchWord = SearchWord;
            }
            int loop = 1;
            while (flag)
            {
                ReportManage.Report(this, loop+"回目", true, true);
                loop++;
                SearchOptions searchOptions = new SearchOptions()
                {
                    Language = this.Language,                  
                    GeoCode = this.GeoCode,
                    Count = 100,
                    ResultType = this.ResultType,                
                    UntilDate = this.UntilDate,

                };
                if (minId != decimal.MaxValue) searchOptions.MaxId = minId-1 ;


                //if (searchWordSinceIdDic.ContainsKey(searchWord))
                //{
                //    searchOptions.SinceId = searchWordSinceIdDic[searchWord];
                //}

                TwitterResponse<TwitterSearchResultCollection> result = null;
                int count = 0;
                while (result == null && count<1)
                {
                    try
                    {
                        if (login != null)
                        {
                            result = Twitterizer.TwitterSearch.Search(login.Token, searchWord, searchOptions);
                        }
                        else
                        {
                            result = Twitterizer.TwitterSearch.Search(searchWord, searchOptions);
                        }
                    }
                    catch
                    {
                        ReportManage.ErrReport(this, "Searchに失敗しました。:"+searchWord);
                      
                    }
                    count++;
                }
                int c = 0;
                int d = 0;
                if (result !=null && result.ResponseObject != null)
                {
                    //Jsonを下流に流す
                    foreach (var item in result.ResponseObject)
                    {
                        maxId = Math.Max(maxId, item.Id);
                        minId = Math.Min(minId, item.Id);
                    
                        if (searchWordDic.ContainsKey(searchWord))
                        {
                            if (searchWordDic[searchWord].Item1 < item.Id || searchWordDic[searchWord].Item2 > item.Id)
                            {
                                c++;
                                yield return TweetData.ConvertXAML(item);
                            }
                            else
                            {
                                d++;
                            }
                        }
                        else
                        {
                            c++;
                           // yield return Codeplex.Data.DynamicJson.Serialize(item);
                            yield return TweetData.ConvertXAML(item);
                        }
                    }


                    //foreach (var item in (dynamic[])Codeplex.Data.DynamicJson.Parse(result.Content))
                    //{
                    //    var id = decimal.Parse(item.id_str);
                    //    maxId = Math.Max(maxId, id);
                    //    minId = Math.Min(minId, id);
                    //    d++;
                    //    if (searchWordDic[searchWord].Item1 < id && searchWordDic[searchWord].Item2 > id)
                    //    {
                    //        c++;
                    //        yield return item.ToString();
                    //    }
                    //} 
                    //すでに取得したものを含むとき
                    if (d >0 || c == 0)
                    {
                        flag = false;
                    }

                    //foreach (var item in result.ResponseObject)
                    //{
                    //    maxId = Math.Max(maxId, item.Id);
                    //    minId = Math.Min(minId, item.Id);

                    //    TweetData t = null;
                    //    try
                    //    {
                    //        t = new TweetData()
                    //        {
                    //            Id = item.Id,
                    //            UsetId = item.User.Id.ToString(),
                    //            Text = item.Text,
                    //            Source = item.Source,
                    //            ScreenName = item.User.ScreenName,
                    //            Annotations = item.Annotations,
                    //            Date = item.CreatedDate,
                    //            Language = item.User.Language,
                    //            Location = item.User.Location,
                    //            ProfileImageLocation = item.User.ProfileImageLocation                          
                    //        };

                    //    }
                    //    catch
                    //    {
                    //        ReportManage.ErrReport(this, "TweetDataのパースに失敗しました。");
                    //    }
                    //    //  if (t != null) yield return System.Xaml.XamlServices.Save(t);
                    //}
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
