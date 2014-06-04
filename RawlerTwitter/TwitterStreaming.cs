using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using Twitterizer;
using Twitterizer.Streaming;

namespace RawlerTwitter
{
    public class TwitterStreaming:RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<TwitterStreaming>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        public RawlerBase StopTree { get; set; }

        int tryCount = 0;
        bool restart = true;
        System.Threading.Tasks.Task task;
        TwitterStream Stream;
        void StartStream()
        {
            task = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        if (restart)
                        {
                            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
                            if (login == null)
                            {
                                ReportManage.ErrReport(this, "上流にTwitterLoginがありません");
                            }
                            Stream = new TwitterStream(login.Token, "Rawler", new StreamOptions() { Track = GetText().Split(',').ToList(), UseCompression = true });


                            Stream.StartUserStream((x) => { ReportManage.Report(this, "ストリームを開始します", true, true); },
                                (x) =>
                                {
                                    ReportManage.ErrReport(this, "ストリームが停止しました" + x.ToString());

                                    if (StopTree != null)
                                    {
                                        StopTree.SetParent(this);
                                        StopTree.Run();
                                    }
                                    if (x == StopReasons.TwitterServerError || x == StopReasons.WebConnectionFailed)
                                    {
                                        System.Threading.Thread.Sleep(tryCount * tryCount * 1000);
                                    }
                                    else
                                    {
                                        System.Threading.Thread.Sleep(tryCount * 250);
                                    }
                                    restart = true;
                                    tryCount++;
                                }
                                ,
                                    (x) =>
                                    {
                                        tryCount = 0;
                                        Document d = new Document() { TextValue = TweetData.ConvertXAML(x) };
                                        d.SetParent(this);
                                        d.SetChildren(this.Children);
                                        d.Run();
                                    },
                                    null, null, null, null, null);
                            restart = false;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                });
        }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            StartStream();
        }

        public override void Dispose()
        {
            if(task !=null)task.Dispose();
            if (Stream != null) Stream.Dispose();
            base.Dispose();
        }
    }
}
