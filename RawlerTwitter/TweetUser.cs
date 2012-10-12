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
                    dynamic[] test = Codeplex.Data.DynamicJson.Parse(GetData(login,userTimelineOptions));
                    int c = 0;


                    foreach (var item in test)
                    {
                        //TweetData t = new TweetData() {
                        //    Text = SecurityElement.Escape(item.text),
                        //    ScreenName = item.user.screen_name,
                        //    Id = decimal.Parse(item.id_str),
                        //    Location = item.user.location,
                        //    UsetId = item.user.id_str
                        //};
                        //t.SetDate(item.created_at);
                        c++;
                        yield return item.ToString();
                    }
                    if (c < userTimelineOptions.Count * 0.9) break;
                        userTimelineOptions.Page++;
                        if (maxPage < userTimelineOptions.Page)
                        {
                            break;
                        }

                    //if (lines.Result == RequestResult.Success)
                    //{
                    //    foreach (var item in lines.ResponseObject)
                    //    {
                    //        TweetData t = null;
                    //        t = new TweetData()
                    //  {
                    //      Id = (long)item.Id,
                    //      UsetId = item.User.Id.ToString(),
                    //      Text = item.Text,
                    //      Source = item.Source,
                    //      ScreenName = item.User.ScreenName,
                    //      Annotations = item.Annotations,
                    //      Date = item.CreatedDate,
                    //      Language = item.User.Language,
                    //      Location = item.Place.FullName,
                    //      ProfileImageLocation = item.User.ProfileImageLocation
                    //  };
                    //        if (t != null) yield return System.Xaml.XamlServices.Save(t);
                    //    }

                    //}
                    //else
                    //{

                    //    ReportManage.ErrReport(this, "TweetUserTimeline:" + lines.Result.ToString() + ":" + lines.ErrorMessage);
                    //    break;
                    //}

                }

            }
            else
            {
                ReportManage.ErrReport(this, "TwitterLoginをTweetUserTimelineの上流に配置してください");
            }
        }


        private string GetData(TwitterLogin login, UserTimelineOptions userTimelineOptions)
        {
            var lines = Twitterizer.TwitterTimeline.UserTimeline(login.Token, userTimelineOptions);
            dynamic[] test = Codeplex.Data.DynamicJson.Parse(lines.Content);

            if (test.Length == 2)
            {
                if (lines.Content.Contains("error"))
                {
                    ReportManage.ErrReport(this, "ツイッター制限で休止中");
                    System.Threading.Thread.Sleep(new TimeSpan(0, 5, 0));
                    return GetData(login, userTimelineOptions);
                }
            }

            return lines.Content;
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
