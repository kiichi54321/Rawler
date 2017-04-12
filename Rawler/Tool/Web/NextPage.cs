using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    /// <summary>
    /// 親の文字列にあるURLとして、NextPage開く。
    /// </summary>
    [ContentProperty("Children")]
    [Serializable]
    public class NextPage : RawlerBase
    {
        public NextPage()
            : base()
        {
        }

        public NextPage(int maxCount)
            : base()
        {
            this.maxCount = maxCount;
        }



        int count = 0;
        int maxCount = int.MaxValue;

        /// <summary>
        /// ページの最大取得数
        /// </summary>
        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }

        int sleepTime = 0;

        public int SleepSecondTime
        {
            get { return sleepTime; }
            set { sleepTime = value; }
        }

        public override string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.Text;
                }
                return string.Empty;
            }

        }

        private Page GetPage()
        {
            Page page = null;
            IRawler current = this.Parent;
            while (current != null)
            {
                if (current is Page)
                {
                    page = current as Page;
                    break;
                }
                current = current.Parent;
            }
            return page;
        }

        HashSet<string> urlHash = new HashSet<string>();
        string baseUrl = string.Empty;

        bool allowSameUrl = false;

        public bool AllowSameUrl
        {
            get { return allowSameUrl; }
            set { allowSameUrl = value; }
        }

        public override void Run(bool runChildren)
        {
            var page = GetPage();
            
            var u = GetText().Replace("&#","&&&&").Split('#');
            string url = string.Empty;
            if (u.Length > 0)
            {
                url = u[0].Replace("&&&&","&#");
            }


            if (this.Parent.Text != null && this.Parent.Text.Length > 0)
            {
                if (page != null)
                {
                    //始まりがURLと違う場合初期化
                    if (page.GetStartUrl() != baseUrl)
                    {
                        baseUrl = page.GetStartUrl();
                        urlHash.Clear();
                        count = 0;
                    }
                    if (allowSameUrl ==false)
                    {
                        if (urlHash.Contains(url) == false)
                        {
                            count++;
                            urlHash.Add(url);
                            if (count < maxCount)
                            {
                                //    this.Text = this.Parent.Text;
                                if (sleepTime > 0)
                                {
                                    System.Threading.Thread.Sleep(new TimeSpan(0, 0, sleepTime));
                                }
                                this.RunChildren(runChildren);
                                ReportManage.Report(this, "NextPage:" + GetText());

                                page.PushUrl(url);
                            }
                            else
                            {
                                ReportManage.Report(this, "NextPage:指定ページ数を取得しました");
                            }
                        }
                    }
                    else
                    {
                        if (count < maxCount)
                        {
                            //    this.Text = this.Parent.Text;
                            if (sleepTime > 0)
                            {
                                System.Threading.Thread.Sleep(new TimeSpan(0, 0, sleepTime));
                            }
                            this.RunChildren(runChildren);
                            ReportManage.Report(this, "NextPage:" + GetText());

                            page.PushUrl(url);
                        }
                        else
                        {
                            ReportManage.Report(this, "NextPage:指定ページ数を取得しました");
                        }
                    }
                }

            }
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<NextPage>(parent);
        }
    }
}
