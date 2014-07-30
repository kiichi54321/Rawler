using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using CoreTweet.Streaming;
using CoreTweet.Streaming.Reactive;
using System.Reactive.Linq;

namespace RawlerTwitter
{
    public class TwitterStreamingFilter : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<TwitterStreamingFilter>(parent);
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
        IDisposable disposable;

        public string Track { get; set; }
        public RawlerBase TrackTree { get; set; }
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
            bool flag = true;
             Dictionary<string, object> dic = new Dictionary<string, object>();
            string track;
            if(TrackTree !=null)
            {
                TrackTree.SetParent(this);
                track = RawlerBase.GetText(GetText(), TrackTree,this);                
            }
            else
            {
                track = Track;
            }
                dic.Add("track",track);
            var stream = login.Token.Streaming.StartObservableStream(StreamingType.Filter,new StreamingParameters(dic)).Publish();

            stream.OfType<StatusMessage>()
                .Subscribe(x => {
                    Document d = new Document() { TextValue = Codeplex.Data.DynamicJson.Serialize(x.Status) };
                    d.SetParent(this);
                    foreach (var item in this.Children)
                    {
                        d.AddChildren(item);
                    }
                    d.Run();
                });
            stream.OfType<WarningMessage>().Subscribe(x => ReportManage.ErrReport(this, x.Message));
            
          ////  stream.OfType<EventMessage>()
          //      .Subscribe(x => Console.WriteLine("{0} by @{1}", x.Event, x.Source.ScreenName));
            disposable = stream.Connect();
        }

        public override void Dispose()
        {  
            base.Dispose();
            if (disposable != null) disposable.Dispose();
        }
    }
}
