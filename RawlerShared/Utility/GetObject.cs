using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
{
    /// <summary>
    /// 
    /// </summary>
    public class GetObject:RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public string ClassName { get; set; }
        public string TargetName { get; set; }


        /// <summary>
        /// このクラスでの実行する内容です。
        /// </summary>
        /// <param name="runChildren"></param>
        public override void Run(bool runChildren)
        {
            var list = this.GetAncestorRawler();

            SetText(string.Empty);
            if (ClassName != null)
            {
                list = list.Where(n => n.ObjectName == ClassName);
                if (list.Count() > 0)
                {
                    SetText(list.First().Text);
                }
                else
                {
                    ReportManage.ErrReport(this, ClassName+"が見つかりませんでした。");
                }
            }
            else if (TargetName != null)
            {
                list = list.Where(n => n.Name == TargetName);
                if (list.Count() > 0)
                {
                    SetText(list.First().Text);
                }
                else
                {
                    ReportManage.ErrReport(this, TargetName + "が見つかりませんでした。");
                }
            }
            else
            {
                ReportManage.ErrReport(this, "ClassName とTargetNameを指定してください。");
            }

            base.Run(runChildren);
        }

        /// <summary>
        /// Text
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetObject>(parent);
        }
    }
}
