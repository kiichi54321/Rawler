using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
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
                var page = this.GetUpperRawler<Page>();
                if (page != null)
                {
                    if (urlType == UrlType.Current)
                    {
                        this.SetText(((Page)page).GetCurrentUrl());
                    }
                    else
                    {
                        this.SetText(((Page)page).GetStartUrl());
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

    /// <summary>
    /// 現在のページのURLをSoruceUrlとしてDataWriteする。
    /// </summary>
    public class DataWriteSoruceUrl : RawlerBase, IDataWrite
    {
        public override void Run(bool runChildren)
        {
            var d = new GetPageUrl().Add(new DataWrite() { Attribute = "SoruceUrl", AttributeType = DataAttributeType.SourceUrl }).GetRoot();
            d.SetParent(this);
            d.Run();
            base.Run(runChildren);
        }

        public string Attribute
        {
            get
            {
                return "SoruceUrl";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DataWriteType WriteType
        {
            get
            {
                return DataWriteType.add;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public enum UrlType
    {
        Current,Start
    }














}
