using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class GetPageUrl:RawlerBase
    {
        UrlType urlType = UrlType.Current;


        public UrlType UrlType
        {
            get { return urlType; }
            set { urlType = value; }
        }

        public override void Run(bool runChildren)
        {
             base.Run(runChildren);          
        }

        public override string Text
        {
            get
            {
                var page = this.GetAncestorRawler().Where(n => n is Page);
                if (page.Count() > 0)
                {
                    if (urlType == Tool.UrlType.Current)
                    {
                        this.SetText(((Page)page.First()).GetCurrentUrl());
                    }
                    else
                    {
                        this.SetText(((Page)page.First()).GetStartUrl());
                    }                  
                }
                else
                {
                    ReportManage.ErrReport(this, "Pageオブジェクトが上流にありません");
                } 
                return base.Text;
            }
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetPageUrl>(parent);
        }
    }

    public enum UrlType
    {
        Current,Start
    }














}
