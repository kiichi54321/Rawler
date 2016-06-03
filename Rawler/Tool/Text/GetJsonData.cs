using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System.Reflection;
using Newtonsoft.Json.Linq;
using RawlerLib.MyExtend;

namespace Rawler.Tool
{
    public class GetJsonData : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetJsonData>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        JToken CurrentJToken = null;

        public JToken GetCurrentJToken()
        {
            return CurrentJToken;
        }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var t = GetText();
            bool flag = false;

            var json = JObject.Parse(t);
            
            if (json.Type == JTokenType.Array)
            {
 
            }
            else
            {
                foreach (var item in FieldName.Split('.'))
                {
                    if (json.Properties().Where(n => n.Name == item).Any())
                    {
                        if (json.Property(item).Value.Type == JTokenType.Object)
                        {
                            json = json.Property(item).Value as JObject;
                     //       json =  JObject.Parse(json.Property(item).Value.ToString());
                        }
                        else
                        {
                            CurrentJToken = json.Property(item).Value;
                     //       SetText(json.Property(item).Value.ToString());
                            flag = true;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                if (json.Type == JTokenType.Object && flag == false)
                {
                    SetText(json.ToString());
                }
                if (flag == false)
                {
                    ReportManage.ErrReport(this, "FieldNameがありません。");
                }
                RunChildren(runChildren);
            }
    
        }

        public string FieldName { get; set; }


        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                if(CurrentJToken !=null)
                {
                    return CurrentJToken.ToString();
                }
                return base.Text;
            }
        }

    }

    /// <summary>
    /// 親テキストをJsonArrayとして読み込みます
    /// </summary>
    public class ReadJsonArray : RawlerMultiBase
    {
        public override void Run(bool runChildren)
        {
            List<string> list = new List<string>();
            JToken json;
            if(this.Parent is GetJsonData)
            {
                json = (this.Parent as GetJsonData).GetCurrentJToken();
            }
            else
            {
                json = JToken.Parse(GetText());
            }
            try
            {
                if(json  is JArray)
                {
                    foreach (var item in json)
                    {
                        list.Add(item.ToString());
                    }
                }

            }
            catch(Exception ex)
            {
                ReportManage.ErrReport(this, "GetJsonAarryに失敗しました。文字列:" + GetText());
            }
            RunChildrenForArray(runChildren, list);
        }
    }


    public class DataWriteJsonData : RawlerBase,IDataWrite
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<DataWriteJsonData>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        /// <summary>
        /// 存在しないFieldNameの場合、エラーを出す。
        /// </summary>
        public bool ErrorThrow { get; set; } = true;

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var t = GetText();
            bool flag = false;
            var json = JObject.Parse(t);
            foreach (var item in FieldName.Split('.'))
            {
                if (json.Properties().Where(n => n.Name == item).Any())
                {
                    if (json.Property(item).Value.Type == JTokenType.Object)
                    {
                        json = JObject.Parse(json.Property(item).Value.ToString());
                    }
                    else
                    {
                        Data.DataWrite(this,FieldName, json.Property(item).Value.ToString(), this.WriteType, DataAttributeType.Text);
                        flag = true;
                    }
                }
                else
                {
                    break;
                }

            }          
            if( json.Type == JTokenType.Object && flag == false)
            {
                Data.DataWrite(this, FieldName, json.ToString(), this.WriteType, DataAttributeType.Text);
            }
            if(flag == false)
            {
                if (ErrorThrow)
                {
                    ReportManage.ErrReport(this, "FieldName:「" + FieldName + "」がありません。");
                }
            }
            base.Run(runChildren);
        }

        public string FieldName { get; set; }


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




        #region IDataWrite メンバー

        public string Attribute
        {
            get
            {
                return FieldName;
            }
            set
            {
                FieldName = value;
            }
        }

        public DataWriteType WriteType
        {
            get;
            set;
        }

        #endregion
    }

    public class DataWriteAllJsonData : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<DataWriteAllJsonData>(parent);
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }
        #endregion

        protected string Json { get; set; }
        protected string PropertyName { get; set; }

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            string t = GetText();
            if (string.IsNullOrEmpty(Json) == false) t = Json;
            var j = JObject.Parse(t);

            foreach (var item in j.Properties())
            {
                if (item.Value != null)
                {
                    if (item.Value.Type == Newtonsoft.Json.Linq.JTokenType.Null)
                    {
                        SetText("Null");
                    }
                    else if (item.Value.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    {
                        DataWriteAllJsonData all = new DataWriteAllJsonData();
                        all.SetParent(this);
                        all.Json = item.Value.ToString();
                        all.PropertyName = this.PropertyName.NullIsEmpty() + item.Name + ".";
                        all.Run();
                    }
                    else
                    {
                        SetText(item.Value.ToString());
                    }
                }
                else
                {
                    SetText("Null");
                }

                DataWrite dataWrite = new DataWrite();
                dataWrite.SetParent(this);
                dataWrite.Attribute = PropertyName.NullIsEmpty()+ item.Name;
                dataWrite.Run();

            }
            base.Run(runChildren);
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

}
