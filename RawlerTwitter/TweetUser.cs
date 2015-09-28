using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using System.Security;
using Newtonsoft.Json.Linq;

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
        public int MaxCount { get; set; }

        public bool ExcludeReplies
        {
            get { return exclude_replies; }
            set { exclude_replies = value; }
        }

        public RawlerBase CompletedTree { get; set; }
        public RawlerBase ErrorTree { get; set; }



        public string ScreenName { get; set; }
        string screenName;
        ParentUserIdType parentTextType = ParentUserIdType.ScreenName;

        public ParentUserIdType ParentUserIdType
        {
            get { return parentTextType; }
            set { parentTextType = value; }
        }

        
        public double SleepSecond { get; set; }
        long max_id = long.MaxValue;


        public IEnumerable<string> ReadData()
        {
            List<string> list = new List<string>();
            var login = this.GetUpperRawler<TwitterLogin>();
            int totalCount = 0;
            
            if (login != null)
            {
               // long max_id = long.MaxValue;
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
                    if (string.IsNullOrEmpty(screenName) == false)
                    {
                        dic.Add("screen_name", screenName);
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
                    long tmp_max_id = max_id;
        
                    bool retry = false;
                    try
                    {
                        foreach (var item in login.Token.Statuses.UserTimeline(dic))
                        {

                            count++;
                            totalCount++;
                            max_id = Math.Min(max_id, item.Id);
                            list.Add(JObject.FromObject(item).ToString());
                            //       yield return JObject.FromObject(item).ToString();
                            if (MaxCount > 0 && MaxCount <= totalCount) break;
                        }
                    }
                    catch (Exception e)
                    {
                      
                        ReportManage.ErrReport(this, GetText() + "\t" + e.Message);
                        if (e.Message == "Not authorized.")
                        {
                            break;
                        }
                        else if (e.Message == "Rate limit exceeded")
                        {
                            retry = true;
                        }
                        else if (e.Message == "Over capacity")
                        {
                            retry = true;
                        }
                        else
                        {
                            throw e;
                        }

                    }

                    if (retry)
                    {
                        ReportManage.Report(this, "3分Sleep", true, true);
                        this.GetUpperRawler<TwitterLogin>().ReLogin();                     
                        System.Threading.Thread.Sleep(new TimeSpan(0, 3, 0));
                        max_id = tmp_max_id;
                    }

                    if (MaxCount > 0 && MaxCount <= totalCount) break;
                    
                    if(count < 10)
                    {
                        break;
                    }
                    if (SleepSecond > 0)
                    {
                        GC.Collect();
                        System.Threading.Thread.Sleep((int)(SleepSecond * 1000));
                    }
                }
            }
            else
            {
                ReportManage.ErrReport(this, "TwitterLoginをTweetUserTimelineの上流に配置してください");
            }
            return list;
        }


  

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            screenName = ScreenName.Convert(this);
            bool flag = true;
            try
            {
                this.RunChildrenForArray(true, ReadData().Distinct());
            }
            catch (Exception e)
            {
                flag = false;
                if (ErrorTree != null && flag == false)
                {
                    ErrorTree.SetParent(this);
                    ErrorTree.Run();
                }
            }
            if(flag)
            {
                max_id = long.MaxValue;
                if( CompletedTree !=null)
                {
                    CompletedTree.SetParent(this.Parent);
                    CompletedTree.Run();
                }
            }
            GC.Collect();
        }

    }

    public enum ParentUserIdType
    {
        ScreenName, UserId
    }
}
