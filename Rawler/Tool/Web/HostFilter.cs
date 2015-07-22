using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 親の文字列はURLなのか？
    /// </summary>
    public class IsUrl:RawlerBase
    {
        bool result = true;

        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        public override void Run(bool runChildren)
        {
            Uri u = null;
            bool r = true;
            if (Uri.TryCreate(GetText(), UriKind.Absolute, out u))
            {
                r = true;
            }
            else
            {
                r = false;
            } 
            if(r == result)
            {
                SetText(GetText());
                base.Run(runChildren);
            }
        }
    }


    /// <summary>
    /// Urlのホスト名があっているかを調べます。
    /// </summary>
    public class HostFilter : RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// このオブジェクトのテキスト
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }

        }

        string hostName = string.Empty;

        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }

        public override void Run(bool runChildren)
        {
            Uri u = null;
            
            if (Uri.TryCreate(GetText(), UriKind.Absolute, out u))
            {
                if (u.Host.Contains(hostName))
                {
                    base.Run(runChildren);
                }
            }
            else
            {
                ReportManage.ErrReport(this, "親の文字列をURIとして変換できませんでした。");
            }
        }



        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<HostFilter>(parent);
        }
    }
}
