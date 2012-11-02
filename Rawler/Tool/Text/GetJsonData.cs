using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;


namespace Rawler.Tool
{
    public class GetJsonData : RawlerMultiBase
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

        /// <summary>
        /// このクラスでの実行すること。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var t = GetText();
            object obj;
            Codeplex.Data.DynamicJson data = Codeplex.Data.DynamicJson.Parse(t);
            if (data.IsArray)
            {
                dynamic[] array = Codeplex.Data.DynamicJson.Parse(t);
                List<string> list = new List<string>();
                foreach (var item in array)
                {
                    list.Add(item.ToString());
                }
                RunChildrenForArray(runChildren, list);
            }
            else
            {
                if (data.TryGetMember(FieldName, out obj))
                {
                    this.SetText(obj.ToString());
                    base.Run(runChildren);
                }
                else
                {
                    ReportManage.ErrReport(this, "FieldNameがありません。");
                }
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
                return base.Text;
            }
        }



    }
}
