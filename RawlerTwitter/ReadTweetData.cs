using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTwitter
{
    public class ReadTweetData : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ReadTweetData>(parent);
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
            var xaml = GetText();

            try
            {
                var td = TweetData.Parse(xaml);
                this.SetText(td.GetTweetDataElement(DataElement));
                base.Run(runChildren);
         
                //var obj = System.Xaml.XamlServices.Parse(xaml);
                //if (obj is TweetData)
                //{
                //    var td = obj as TweetData;
                //    this.SetText(td.GetTweetDataElement(DataElement));
                //    base.Run(runChildren);
                //}
                //else
                //{
                //    ReportManage.ErrReport(this, "TweetDataの解釈に失敗しました。");
                //}
            }
            catch
            {
                ReportManage.ErrReport(this, "TweetDataの解釈に失敗しました。");
            }

        }

        public TweetData.TweetDataElements DataElement
        {
            get;
            set;
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
