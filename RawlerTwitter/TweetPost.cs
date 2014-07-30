using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTwitter
{
    public class TweetPost : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<TweetPost>(parent);
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
            SetText(GetText());
            var login = this.GetAncestorRawler().OfType<TwitterLogin>().First();
            if (login != null)
            {


                var r = login.Token.Statuses.Update(status => GetText());
                if (r !=null )
                {
                    // Tweet posted successfully!
                }
                else
                {
                    ReportManage.ErrReport(this, "ツイートに失敗しました。");
                    // Something bad happened
                }
            }
            else
            {
                ReportManage.ErrReport(this, "上流にTwitterLoginがありません。");               
            }




            base.Run(runChildren);
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
