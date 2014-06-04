using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using Twitterizer;
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

        public RawlerBase ErrorTree { get; set; }

        int count = 20;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        int maxPage = 20;

        public int MaxPage
        {
            get { return maxPage; }
            set { maxPage = value; }
        }

        public string ScreenName { get; set; }


        public IEnumerable<string> ReadData()
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();

            if (login != null)
            {
                Dictionary<string, string> query = new Dictionary<string, string>();
                var userTimelineOptions = new UserTimelineOptions();
                userTimelineOptions.Count = count;
                userTimelineOptions.IncludeRetweets = includeRetweets;
                userTimelineOptions.UseSSL = true;

                if (ScreenName == null || ScreenName.Length == 0)
                {
                    userTimelineOptions.ScreenName = GetText();
                }
                else
                {
                    userTimelineOptions.ScreenName = ScreenName;
                }
                userTimelineOptions.UserId = 0;
                userTimelineOptions.Page = 1;



                while (true)
                {
                    var r = GetData(login, userTimelineOptions);
                    if (r.Item1 == false)
                    {
                        if (ErrorTree != null)
                        {
                            this.SetText(GetText() + "\t" + r.Item2);
                            ErrorTree.SetParent(this);
                            ErrorTree.Run();
                        }

                        break;
                    }
                //    dynamic[] test = Codeplex.Data.DynamicJson.Parse(r.Item2);
                    int c = 0;


                    foreach (var item in r.Item2)
                    {
                        c++;
                        yield return TweetData.ConvertXAML(item);
                        //yield return Codeplex.Data.DynamicJson.Serialize(item);
                    }
                    if (c < userTimelineOptions.Count * 0.9) break;
                    userTimelineOptions.Page++;
                    if (maxPage < userTimelineOptions.Page)
                    {
                        break;
                    }
                }
            }
            else
            {
                ReportManage.ErrReport(this, "TwitterLoginをTweetUserTimelineの上流に配置してください");
            }
        }


        private Tuple<bool,TwitterStatusCollection> GetData(TwitterLogin login, UserTimelineOptions userTimelineOptions)
        {
            var lines = Twitterizer.TwitterTimeline.UserTimeline(login.Token, userTimelineOptions);
            dynamic[] test = Codeplex.Data.DynamicJson.Parse(lines.Content);

            if (test.Length == 2)
            {
                if (lines.Content.Contains("error"))
                {
                    ReportManage.ErrReport(this, test[1].ToString());
                    if (lines.Content.Contains("Not authorized"))
                    {
                        return new Tuple<bool, TwitterStatusCollection>(false, new TwitterStatusCollection());
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(new TimeSpan(0, 10, 0));
                        return GetData(login, userTimelineOptions);
                    }
                }
            }

            return new Tuple<bool, TwitterStatusCollection>(true, lines.ResponseObject);
        }


        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            this.RunChildrenForArray(true, ReadData());
        }

    }

}
