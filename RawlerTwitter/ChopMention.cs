
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerTwitter
{
    public class ChopMention : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<ChopMention>(parent);
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
            string str = GetText();

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"@[^\s]*");
            str = regex.Replace(str, " ");

            SetText(str);


            base.Run(runChildren);
        }
    }

    public class GetMention : RawlerMultiBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<GetMention>(parent);
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
            string str = GetText();

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"@[^\s]*");
            str = regex.Replace(str, " ");
            List<string> list = new List<string>();
            foreach (System.Text.RegularExpressions.Match item in regex.Matches(str))
            {
                list.Add(item.Value);
            }
            base.RunChildrenForArray(runChildren,list);
        }
    }
}

