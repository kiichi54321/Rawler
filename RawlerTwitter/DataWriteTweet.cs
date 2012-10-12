using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTwitter
{
    public class DataWriteTweet : RawlerBase,IDataWrite
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<DataWriteTweet>(parent);
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
        /// 追加か書き換えかを指定します。
        /// </summary>
        public DataWriteType WriteType
        {
            get { return writeType; }
            set { writeType = value; }
        }
        DataWriteType writeType = DataWriteType.add;

        private string attribute = null;
        public  string Attribute
        {
            get
            {
                if (string.IsNullOrEmpty(attribute))
                {
                    attribute = DataElement.ToString();
                }
                return attribute;
            }
            set
            {
                attribute = value;
            }
        }


        


        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (this.Parent == null)
            {
                ReportManage.ErrReport(this, "親クラスがありません。");
                return; 
            }
            var xaml = this.Parent.Text;

        
            try
            {
                var td = TweetData.Parse(xaml);
                this.SetText(td.GetTweetDataElement(DataElement));

                //var obj = System.Xaml.XamlServices.Parse(xaml);
                //if (obj is TweetData)
                //{
                //    var td = obj as TweetData;
                //    var t = td.GetTweetDataElement(DataElement);
                //    t = this.GetText(t);
                //    this.SetText(t);

                var data = this.GetAncestorRawler().OfType<Data>().First();
                if (data != null)
                {
                    data.DataWrite(Attribute,this.GetText(this.text), writeType);
                }
                else
                {
                    ReportManage.ErrReport(this, "上流にDataクラスがありません。");
                }


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
