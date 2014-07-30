using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using System.Security;

namespace RawlerTwitter
{
    public class TweetUserTimeline : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<TweetUserTimeline>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion


        bool includeRetweets = true;

        public bool IncludeRetweets
        {
            get { return includeRetweets; }
            set { includeRetweets = value; }
        }
        bool exclude_replies = true;

        public bool ExcludeReplies
        {
            get { return exclude_replies; }
            set { exclude_replies = value; }
        }

        public RawlerBase ErrorTree { get; set; }

        //int count = 20;

        //public int Count
        //{
        //    get { return count; }
        //    set { count = value; }
        //}

        //int maxPage = 20;

        //public int MaxPage
        //{
        //    get { return maxPage; }
        //    set { maxPage = value; }
        //}

        public string ScreenName { get; set; }
        ParentUserIdType parentTextType = ParentUserIdType.ScreenName;

        public ParentUserIdType ParentUserIdType
        {
            get { return parentTextType; }
            set { parentTextType = value; }
        }
        



        public IEnumerable<string> ReadData()
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();

            if (login != null)
            {
                long max_id = long.MaxValue;
                while (true)
                {
                    /// <para>- <c>long</c> user_id (optional)</para>
                    /// <para>- <c>string</c> screen_name (optional)</para>
                    /// <para>- <c>int</c> count (optional)</para>
                    /// <para>- <c>long</c> since_id(optional)</para>
                    /// <para>- <c>long</c> max_id (optional)</para>
                    /// <para>- <c>bool</c> trim_user (optional)</para>
                    /// <para>- <c>bool</c> contributor_details (optional)</para>
                    /// <para>- <c>bool</c> include_rts (optional)</para>
                    /// <para>- <c>bool</c> exclude_replies (optional)</para>

                    Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                       {"include_rts", IncludeRetweets},
                       {"exclude_replies", exclude_replies},                    
                       {"count", 100}                                
                };
                    if (string.IsNullOrEmpty(ScreenName) == false)
                    {
                        dic.Add("screen_name", ScreenName);
                    }
                    else
                    {
                        if (ParentUserIdType == RawlerTwitter.ParentUserIdType.ScreenName)
                        {
                            dic.Add("screen_name", GetText());
                        }
                        else
                        {
                            long id;
                            if (long.TryParse(GetText(), out id))
                            {
                                dic.Add("user_id", long.Parse(GetText()));
                            }
                            else
                            {
                                ReportManage.ErrReport(this, "親テキストがLONG型ではありません。");
                                dic.Add("screen_name", GetText());
                            }

                        }
                    }
                    if (max_id < long.MaxValue)
                    {
                        dic.Add("max_id", max_id - 1);
                    }
                    int count = 0;
                   
                        foreach (var item in login.Token.Statuses.UserTimeline(dic))
                        {
                            count++;
                            max_id = Math.Min(max_id, item.Id);
                            yield return Codeplex.Data.DynamicJson.Serialize(item);
                        }
                   
                    
                    if(count < 90)
                    {
                        break;
                    }
                }


                //while (true)
                //{
                //    var r = GetData(login, userTimelineOptions);
                //    if (r.Item1 == false)
                //    {
                //        if (ErrorTree != null)
                //        {
                //            this.SetText(GetText() + "\t" + r.Item2);
                //            ErrorTree.SetParent(this);
                //            ErrorTree.Run();
                //        }

                //        break;
                //    }
                ////    dynamic[] test = Codeplex.Data.DynamicJson.Parse(r.Item2);
                //    int c = 0;


                //    foreach (var item in r.Item2)
                //    {
                //        c++;
                //        yield return TweetData.ConvertXAML(item);
                //        //yield return Codeplex.Data.DynamicJson.Serialize(item);
                //    }
                //    if (c < userTimelineOptions.Count * 0.9) break;
                //    userTimelineOptions.Page++;
                //    if (maxPage < userTimelineOptions.Page)
                //    {
                //        break;
                //    }
               // }
            }
            else
            {
                ReportManage.ErrReport(this, "TwitterLoginをTweetUserTimelineの上流に配置してください");
            }
        }


        //private Tuple<bool,TwitterStatusCollection> GetData(TwitterLogin login, UserTimelineOptions userTimelineOptions)
        //{
        //    var lines = Twitterizer.TwitterTimeline.UserTimeline(login.Token, userTimelineOptions);
        //    dynamic[] test = Codeplex.Data.DynamicJson.Parse(lines.Content);

        //    if (test.Length == 2)
        //    {
        //        if (lines.Content.Contains("error"))
        //        {
        //            ReportManage.ErrReport(this, test[1].ToString());
        //            if (lines.Content.Contains("Not authorized"))
        //            {
        //                return new Tuple<bool, TwitterStatusCollection>(false, new TwitterStatusCollection());
        //            }
        //            else
        //            {
        //                System.Threading.Thread.Sleep(new TimeSpan(0, 10, 0));
        //                return GetData(login, userTimelineOptions);
        //            }
        //        }
        //    }

        //    return new Tuple<bool, TwitterStatusCollection>(true, lines.ResponseObject);
        //}


        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            try
            {
                this.RunChildrenForArray(true, ReadData());
            }
            catch (Exception e)
            {
                ReportManage.ErrReport(this, GetText() + "\t" + e.Message);
                ErrorTree.SetParent(this);
                ErrorTree.Run();
            }
        }

    }

    public enum ParentUserIdType
    {
        ScreenName, UsetId
    }
}
