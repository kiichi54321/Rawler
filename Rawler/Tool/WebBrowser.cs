using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class WebBrowser:WebClient
    {
        public WebBrowser()
            :base()
        {
            browser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(browser_LoadCompleted);
        }

        bool LoadCompleted = false;
        void browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            LoadCompleted = true;
        }
        private System.Windows.Controls.WebBrowser browser = new System.Windows.Controls.WebBrowser();

        public override string HttpGet(string url, Encoding enc)
        {
            LoadCompleted = false;

            browser.Navigate(url);

            while (LoadCompleted == false)
            {
                System.Threading.Thread.Sleep(100);
            }
//            browser.com
            dynamic doc = browser.Document;
            dynamic body = doc.body;



            return body.innerHTML;
        }
    }


}
