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
    public class AppOnlyAuthentication
    {
        public AppOnlyAuthentication()
        {
        }

        public void SetUp()
        {
            if (string.IsNullOrEmpty(SetTwitterApiKeys.consumerKey) || string.IsNullOrEmpty(SetTwitterApiKeys.consumerSecret))
            {
                this.ConsumerKey = "gHVupgapEXlTZdu7rf3oOg";
                this.ConsumerSecret = "YOicLtW8utx3NJyy88wtzq8QN3ilXeQoEGCPIJNzo";
                ReportManage.Report(null, "RawlerのAPI Keyを使います",true,true);
            }
            else
            {
                this.ConsumerKey = SetTwitterApiKeys.consumerKey;
                this.ConsumerSecret = SetTwitterApiKeys.consumerSecret;
            } 
        }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }

    /// <summary>
    /// TwitterApiキーを設定する。
    /// </summary>
    public class SetTwitterApiKeys:RawlerBase
    {
        public static string consumerKey { get; set; }
        public static string consumerSecret { get; set; }

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        static bool useAppOnlyAutherentcation = false;

        public static bool UseAppOnlyAutherentcation
        {
            get { return SetTwitterApiKeys.useAppOnlyAutherentcation; }
            set { SetTwitterApiKeys.useAppOnlyAutherentcation = value; }
        }

        public override void Run(bool runChildren)
        {
            consumerKey = this.ConsumerKey;
            consumerSecret = this.ConsumerSecret;
            useAppOnlyAutherentcation = true;
            base.Run(runChildren);
        }
    }

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
  
        protected CoreTweet.Core.TokensBase token;
        public CoreTweet.Core.TokensBase Token { get { return token; } protected set { token = value; } }
        public TokenData TokenData { get; set; }
        public AppOnlyAuthentication AppOnlyAuthentication { get; set; }


        private string tokenSettingFileName = "RawlerTwitterToken.setting";
        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            if (SetTwitterApiKeys.UseAppOnlyAutherentcation || AppOnlyAuthentication != null)
            {
                if(AppOnlyAuthentication == null)
                {
                    AppOnlyAuthentication = new AppOnlyAuthentication();                    
                }
                AppOnlyAuthentication.SetUp();
                token = OAuth2.GetToken(AppOnlyAuthentication.ConsumerKey, AppOnlyAuthentication.ConsumerSecret);
            }
            else
            {
                if (GetOAuthTokens() == null)
                {
                    var session = OAuth.Authorize(ConsumerKey, ConsumerSecret);
                    ReportManage.Report(this, "PINがありません。ブラウザに表示されているPINの値を入力してください。PIN=\"(表示されているPIN)\"　とTwitterLoginクラスに情報を追加してください");
                    System.Diagnostics.Process.Start(session.AuthorizeUri.ToString());

                    Task reportProgressTask = Task.Factory.StartNew(() =>
                    {
                        PinDialog pin = new PinDialog();
                        if (pin.ShowDialog() == true)
                        {
                            var tokens = session.GetTokens(pin.PIN);


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
            }
            base.Run(runChildren);
        }

        public void ReLogin()
        {            
            token = null;
            GC.Collect();
            if (AppOnlyAuthentication != null)
            {
                token = OAuth2.GetToken(AppOnlyAuthentication.ConsumerKey, AppOnlyAuthentication.ConsumerSecret);
            }
            else
            {
                if (GetOAuthTokens() == null)
                {
                    var session = OAuth.Authorize(ConsumerKey, ConsumerSecret);
                    ReportManage.Report(this, "PINがありません。ブラウザに表示されているPINの値を入力してください。PIN=\"(表示されているPIN)\"　とTwitterLoginクラスに情報を追加してください");
                    System.Diagnostics.Process.Start(session.AuthorizeUri.ToString());

                    Task reportProgressTask = Task.Factory.StartNew(() =>
                    {
                        PinDialog pin = new PinDialog();
                        if (pin.ShowDialog() == true)
                        {
                            var tokens = session.GetTokens(pin.PIN);


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
            }
        }


        public CoreTweet.Core.TokensBase GetOAuthTokens()
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
