using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Rawler
{
    /// <summary>
    /// 上流にある「Dataクラス」のDataRowに親のテキストを書き込むためのRawlerクラス。
    /// </summary>

    public class DataWrite : RawlerBase,IDataWrite
    {

        /// <summary>
        /// 上流にある「Dataクラス」のDataRowに親のテキストを書き込むためのRawlerクラス。
        /// </summary>
        public DataWrite()
            : base()
        {
          
        }

        /// <summary>
        /// 上流にある「Dataクラス」のDataRowに親のテキストを書き込むためのRawlerクラス。
        /// </summary>
        /// <param name="attribute"></param>
        public DataWrite(string attribute)
            : base()
        {
            
            this.Attribute = attribute;
        }


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
                return GetText(); 
            }

        }

        private bool useHtmlDecode = true;

        /// <summary>
        /// HtmlDecode を使う。デフォルトではTrue
        /// </summary>
        public bool UseHtmlDecode
        {
            get { return useHtmlDecode; }
            set { useHtmlDecode = value; }
        }

        /// <summary>
        /// 書き込むときに使うAttribute
        /// </summary>
        public virtual string Attribute { get; set; }

        private DataAttributeType attributeType = DataAttributeType.Text;

        public DataAttributeType AttributeType
        {
            get { return attributeType; }
            set { attributeType = value; }
        } 

        ///// <summary>
        ///// 書き込むときに使うAttribute 
        ///// </summary>
        //public string AttributeObjectName { get; set; }

        DataWriteType writeType = DataWriteType.add;

        /// <summary>
        /// 追加か書き換えかを指定します。
        /// </summary>
        public DataWriteType WriteType
        {
            get { return writeType; }
            set { writeType = value; }
        }


        RawlerBase attributeTree = null;

        /// <summary>
        /// 属性部分を生成するTreeです。
        /// </summary>
        public RawlerBase AttributeTree
        {
            get { return attributeTree; }
            set { attributeTree = value; }
        }

        public string Value { get; set; }



        //public new void Run()
        //{
        //    Run(true);
        //}

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            string txt = string.Empty;
            if (Value == null || Value.Length == 0)
            {
                txt = GetText();
            }
            else
            {
                txt = Value.Convert(this);
            }
            if (useHtmlDecode)
            {
                txt = System.Net.WebUtility.HtmlDecode(txt);
            }

            string attribute = string.Empty;
            if (this.Attribute == null && attributeTree != null)
            {               
                if (this.Parent != null)
                {
                    attribute = RawlerBase.GetText(this.Parent.Text, attributeTree, this);
                }
            }
            else if(this.Attribute !=null)
            {
                attribute = this.Attribute.Convert(this);
            }

            Data.DataWrite(this,attribute,txt, writeType, AttributeType);

            //IData data = null;

            //IRawler current = this.Parent;
            //while (current != null)
            //{ 
            //    if (current is IData)
            //    {
            //        data = current as IData;
            //        break;
            //    }
            //    current = current.Parent;
            //}
            //if (data != null)
            //{
            //    string txt = string.Empty;
            //    if (Value == null || Value.Length == 0)
            //    {
            //        txt = GetText();
            //    }
            //    else
            //    {
            //        txt = Value;
            //    }
            //    if (useHtmlDecode)
            //    {
            //        txt = System.Net.WebUtility.HtmlDecode(txt);
            //    }

            //    if (this.Attribute == null && attributeTree != null)
            //    {
            //        string tmpAttributeText = string.Empty;
            //        if (this.Parent != null)
            //        {
            //            tmpAttributeText = RawlerBase.GetText(this.Parent.Text, attributeTree,this);

            //        }
            //        data.DataWrite(tmpAttributeText, txt, writeType,AttributeType);

            //    }
            //    else
            //    {
            //        if (this.Attribute != null)
            //        {
            //            data.DataWrite(this.Attribute, txt, writeType,AttributeType);
            //        }
            //        else
            //        {
            //            data.DataWrite(string.Empty, txt, writeType, AttributeType);
            //        }
            //    }

            //    //if (this.AttributeObjectName != null && this.AttributeObjectName.Length>0)
            //    //{
            //    //    var list = this.GetConectAllRawler().Where(n => n.Name == this.AttributeObjectName);
            //    //    if (list.Count() > 0)
            //    //    {
            //    //        data.DataWrite(list.First().Text, txt,writeType);
            //    //    }
            //    //    else
            //    //    {
            //    //        ReportManage.ErrReport(this, "AttributeObjectNameの指定が不正です。オブジェクトが見つかりませんでした。");
            //    //    }
            //    //}

            //}
            //else
            //{
            //    ReportManage.ErrReport(this,"書き込み先のDataオブジェクトが見つかりません。");
            //}
            this.RunChildren(runChildren);
        }
        
    }

    public enum DataWriteType
    {
        add,replace
    }
}
