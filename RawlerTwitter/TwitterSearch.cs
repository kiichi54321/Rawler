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

        private Dictionary<string, decimal> searchWordIdDic = new Dictionary<string, decimal>();
        public string Language { get; set; }
        public string GeoCode { get; set; }
        public DateTime SinceDate { get; set; }
        public DateTime UntilDate { get; set; }
        public string SearchWord { get; set; }

        protected IEnumerable<string> ReadSearch()
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
            bool flag = true;
            int pageCount = 1;
            decimal maxid = -1;
            string searchWord = GetText();
            if (string.IsNullOrEmpty(SearchWord) == false)
            {
                searchWord = SearchWord;
            }
            while (flag)
            {
                SearchOptions searchOptions = new SearchOptions()
                {
                    Language = this.Language,
                    PageNumber = pageCount,
                    GeoCode = this.GeoCode,
                    NumberPerPage = 100,
                    SinceDate = this.SinceDate,
                    UntilDate = this.UntilDate,

                };
                if (maxid > 0) searchOptions.MaxId = maxid;


                if (searchWordIdDic.ContainsKey(searchWord))
                {
                    searchOptions.SinceId = searchWordIdDic[searchWord];
                }

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
                if (result !=null && result.ResponseObject != null)
                {
                    foreach (var item in (dynamic[])Codeplex.Data.DynamicJson.Parse(result.Content))
                    {
                        yield return item.ToString();
                    } ;

                    foreach (var item in result.ResponseObject)
                    {
                        maxid = Math.Max(maxid, item.Id);
                        c++;
                        TweetData t = null;
                        try
                        {
                            t = new TweetData()
                            {
                                Id = item.Id,
                                UsetId = item.FromUserId.ToString(),
                                Text = item.Text,
                                Source = item.Source,
                                ScreenName = item.FromUserScreenName,
                                Annotations = item.Annotations,
                                Date = item.CreatedDate,
                                Language = item.Language,
                                Location = item.Location,
                                ProfileImageLocation = item.ProfileImageLocation

                            };

                        }
                        catch
                        {
                            ReportManage.ErrReport(this, "TweetDataのパースに失敗しました。");
                        }
                      //  if (t != null) yield return System.Xaml.XamlServices.Save(t);
                    }
                }
                else
                {
                    flag = false;
                }

                pageCount++;

                if (searchWordIdDic.ContainsKey(searchWord))
                {
                    searchWordIdDic[searchWord] = maxid;
                }
                else
                {
                    searchWordIdDic.Add(searchWord, maxid);
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
