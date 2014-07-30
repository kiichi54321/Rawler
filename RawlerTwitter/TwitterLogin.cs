using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using System.Threading.Tasks;
using System.Threading;
using CoreTweet;

namespace RawlerTwitter
{
    public class TwitterLogin : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<TwitterLogin>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        string ConsumerKey = "gHVupgapEXlTZdu7rf3oOg";
        string ConsumerSecret = "YOicLtW8utx3NJyy88wtzq8QN3ilXeQoEGCPIJNzo";

  
        protected Tokens token;
        public Tokens Token { get { return token; } protected set { token = value; } }
        public TokenData TokenData { get; set; }

        private string tokenSettingFileName = "RawlerTwitterToken.setting";
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {

          //  OAuth2Token apponly = OAuth2.GetToken("consumer key", "consumer secret");

               if (GetOAuthTokens() == null)
            {
                var session = OAuth.Authorize(ConsumerKey, ConsumerSecret);
         //       Console.WriteLine("access here: {0}", session.AuthorizeUri);
               // var tokens = session.GetTokens("PINCODE");

                      ReportManage.Report(this, "PINがありません。ブラウザに表示されているPINの値を入力してください。PIN=\"(表示されているPIN)\"　とTwitterLoginクラスに情報を追加してください");
                      System.Diagnostics.Process.Start(session.AuthorizeUri.ToString());



                Task reportProgressTask = Task.Factory.StartNew(() =>
                {
                    PinDialog pin = new PinDialog();
                    if (pin.ShowDialog() == true)
                    {
                    var    tokens = session.GetTokens(pin.PIN);
                  

                        TokenData td = new TokenData()
                        {
                            ConsumerKey = ConsumerKey,
                            ConsumerSecret = ConsumerSecret,
                            AccessToken = tokens.AccessToken,
                            AccessTokenSecret = tokens.AccessTokenSecret
                        };
                        this.Token = new Tokens(tokens);
                        this.TokenData = td;
                        System.Xaml.XamlServices.Save(tokenSettingFileName, td);
                    }
                    else
                    {
                        return;
                    }

                },
         CancellationToken.None,
         TaskCreationOptions.None,
         RawlerLib.UIData.UISyncContext);
                reportProgressTask.Wait();

            }
            base.Run(runChildren);
        }

        public Tokens GetOAuthTokens()
        {
            if (System.IO.File.Exists(tokenSettingFileName))
            {
                try
                {
                    var t = System.Xaml.XamlServices.Load(tokenSettingFileName);
                    if (t is TokenData)
                    {
                        var td = t as TokenData;

                        token = new Tokens()
                        {
                            AccessToken = td.AccessToken,
                            AccessTokenSecret = td.AccessTokenSecret,
                            ConsumerKey = td.ConsumerKey,
                            ConsumerSecret = td.ConsumerSecret
                        };
                         
                        this.TokenData = t as TokenData;
                    }
                    else
                    {
                        ReportManage.ErrReport(this, tokenSettingFileName + "の形式がおかしいみたいです。");
                        token = null;
                    }
                }
                catch (Exception e)
                {
                    ReportManage.ErrReport(this, tokenSettingFileName + "を開くのに失敗しました" + e.Message);
                    token = null;
                }
            }
            else
            {
                token = null;
            }
            return token;
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


    public class TokenData
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
