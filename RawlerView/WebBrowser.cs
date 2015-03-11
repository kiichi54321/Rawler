using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RawlerView
{
    public class WebBrowser:Rawler.Tool.WebClient
    {
        static System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();
        public override string HttpGet(string url, Encoding enc)
        {
            if (enc != null) browser.Document.Encoding = enc.ToString();
            browser.Navigate(url);
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            return browser.Document.Body.InnerHtml;
            ////Added this line, because the final HTML takes a while to show up
            //GeneratedSource = wb.Document.Body.InnerHtml;
            //return base.HttpGet(url, enc);
        }

        public override string HttpPost(string url, List<Rawler.Tool.KeyValue> vals)
        {
            browser.Url = new Uri(url);
            browser.DocumentText = base.HttpPost(url, vals);
            while (browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            return browser.Document.Body.InnerHtml;
        }

        
    }


    
}
