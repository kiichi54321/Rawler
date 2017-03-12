using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    /// <summary>
    /// 再ログインを発生させます。必ず上流にLoginClientが必要です。
    /// </summary>
    public class ReLogin:RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// このオブジェクトのテキスト。親のテキストをそのまま流す。
        /// </summary>
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

        private List<string> loginErrMessages = new List<string>();
        /// <summary>
        /// ログイン失敗時、表示されるメッセージです。これを含んでいる時、再ログインを発生させます。空の時は、調べずに再ログインを発生させます。
        /// </summary>
        public List<String> LoginErrMessages
        {
            get{return loginErrMessages;}
            set{loginErrMessages = value;}
        }


        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            bool flag = false;
            if (loginErrMessages.Count > 0)
            {
                foreach (var item in loginErrMessages)
                {
                    if (this.text.Contains(item))
                    {
                        flag = true;
                    }
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {

                LoginClient loginClient = null;

                IRawler current = this.Parent;
                while (current != null)
                {
                    if (current is LoginClient)
                    {
                        loginClient = current as LoginClient;
                        break;
                    }
                    current = current.Parent;
                }
                if (loginClient != null)
                {
                    //          breakFlag = true;
                    loginClient.ReLogin();
                    var list = this.GetAncestorRawler().Where(n => n is Page);
                    if (list.Count() > 0)
                    {
                        var p = list.First() as Page;
                        p.GetCurrentPage();
                    }
                    else
                    {

                        ReportManage.ErrReport(this, "Pageオブジェクトが上流に見つかりません。");

                    }
                }
                else
                {
                    ReportManage.ErrReport(this, "LoginClientオブジェクトが上流に見つかりません。");
                }
                this.RunChildren(runChildren);
            }
        }


    }
}
