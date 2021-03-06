﻿using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler
{
    public class RandomIntValue : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<RandomIntValue>(parent);
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
            SetText(random.Next(maxValue).ToString());
            base.Run(runChildren);
        }

        int maxValue = 100;

        public int MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        Random random = new Random();

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
