using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawlerPhantom
{
    public class Phantom:Rawler.Tool.WebClient
    {
        PhantomJSDriver driver = null;
        public override void Dispose()
        {
            if (driver != null) driver.Quit();
            base.Dispose();
        }

        public override string HttpGet(string url, Encoding enc)
        {
            if (driver == null)
            {
                var options = new PhantomJSOptions();
                //            var userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                options.AddAdditionalCapability("phantomjs.page.settings.userAgent", this.UserAgent);
                driver = new PhantomJSDriver(options);
            }

            try
            {
                driver.Navigate().GoToUrl(url);
                // 待ち条件を設定
//                Thread.Sleep(2000); // 汎用的な条件は不可能のため任意の秒数待つ(2秒)

                // 【参考】待ち条件の例 (* waitしなくても document.readyState == "complete" は保証される)
                // var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));   // 最大待ち時間(10秒)
                // wait.Until(ExpectedConditions.TitleIs("foo"));   // titleがfoo
                //// wait.Until(d => d.Title == "bar");             // titleがbar

                var innerHtml = "";
                try
                {
                    var element = driver.FindElement(By.TagName("body"));
                    innerHtml = element.GetAttribute("innerHTML");
                }
                catch (NoSuchElementException ignore)
                {
                    Rawler.Tool.ReportManage.ErrReport(this,"HTMLの取得に失敗"+ ignore.Message );
                    // Page which does not have <body> tag is not supported, so catch error and continue.
                }

                return innerHtml;
            }
            finally
            {
                // .Close()では仕様上不十分のためfinallyにて.Quit()で処理
            //    driver.Quit();
            }
//            return base.HttpGet(url, enc);
        }
    }
}
