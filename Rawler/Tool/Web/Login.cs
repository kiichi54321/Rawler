using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{

    /// <summary>
    /// ログインをするサイトでのWebClient
    /// </summary>
    public class LoginClient:WebClient
    {
        bool hasLogin = false;
        string loginPage = string.Empty;

   

        /// <summary>
        /// ログインをするサイトでのWebClient
        /// </summary>
        public LoginClient()
            : base()
        {
        }

        void LoginClient_LoginEvent(object sender, EventArgs e)
        {
            if (hasLogin == false)
            {
                ReLogin();
            }
        }

        private void OnLogin()
        {
            if (hasLogin == false)
            {
                ReLogin();
            }
        }

        public override string HttpGet(string url, Encoding enc)
        {
            OnLogin();
            return base.HttpGet(url, enc);
        }

        public override byte[] HttpGetByte(string url)
        {
            OnLogin();
            return base.HttpGetByte(url);
        }

        /// <summary>
        /// HttpPost オートでログインします。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public override string HttpPost(string url, List<KeyValue> vals)
        {
            OnLogin();
            return base.HttpPost(url, vals);
        }

        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url"></param>
        /// <param name="vals"></param>
        /// <param name="login">ログインする</param>
        /// <returns></returns>
        public string HttpPost(string url, List<KeyValue> vals, bool login)
        {
            if (login)
            {
                OnLogin();
            }
            return base.HttpPost(url, vals);
        }


        /// <summary>
        /// ログインした。
        /// </summary>
        public bool HasLogin
        {
            get
            {
                return hasLogin;
            }
        }

        public override void Run(bool runChildren)
        {
            ReLogin();
            base.Run(runChildren);
        }


        /// <summary>
        /// ログインにつかうページ。
        /// </summary>
        public string LoginUrl
        {
            get { return loginPage; }
            set { loginPage = value; }
        }
        /// <summary>
        /// ログイン失敗時の文字列（ログイン失敗判定に使います。）
        /// </summary>
        public string ErrString { get; set; }
        List<KeyValue> vals = new List<KeyValue>();

        /// <summary>
        /// Loginフォームから送信する情報です。
        /// </summary>
        public List<KeyValue> LoginPostVals
        {
            get { return vals; }
            set { vals = value; }
        }

        //public string Login(string id, string password)
        //{
        //    //// ログイン・ページへのアクセス
        //    //Dictionary<string, string> vals = new Dictionary<string, string>();
        //    //vals.Add("mode", "login");
        //    //vals.Add("pixiv_id", id);
        //    //vals.Add("pass", password);



        //    string login = "http://www.pixiv.net/index.php";
        //    string html = HttpPost(loginPage, vals);


        //    if (cc.Count > 0)
        //    {
        //        hasLogin = true;
        //        MyLib.Log.LogWriteLine("Login成功！");
        //    }

        //    //        html = HttpGet("http://www.pixiv.net/");
        //    return html;
        //}

        private bool isNoCookieSite = false;

        public bool IsNoCookieSite
        {
            get { return isNoCookieSite; }
            set { isNoCookieSite = value; }
        }


        /// <summary>
        /// 再びログインする。
        /// </summary>
        public void ReLogin()
        {
            string html = this.HttpPost(loginPage, vals,false);
            if (this.GetCookieCount() > 0)
            {
                if (ErrString != null)
                {
                    if (html.Contains(ErrString) == false)
                    {
                        hasLogin = true;
                        ReportManage.Report(this, "ログイン成功");
                    }
                    else
                    {
                        hasLogin = false;
                        ReportManage.ErrReport(this, "ログイン失敗");
                    }
                }
                else
                {
                    hasLogin = true;
                    ReportManage.Report(this, "ログイン成功");
                }
            }
            else
            {
                if (isNoCookieSite)
                {
                    hasLogin = false;
                    ReportManage.ErrReport(this, "ログイン失敗");
                }
                else
                {
                    if (ErrString != null)
                    {
                        if (html.Contains(ErrString) == false)
                        {
                            hasLogin = true;
                            ReportManage.Report(this, "ログイン成功");
                        }
                        else
                        {
                            hasLogin = false;
                            ReportManage.ErrReport(this, "ログイン失敗");
                        }
                    }
                    else
                    {
                        hasLogin = true;
                        ReportManage.Report(this, "ログイン成功");
                    }
                }
            }
            this.text = html;
        }
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        /// <summary>
        /// クローンを作る
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<LoginClient>(parent);
        }
    }
}
