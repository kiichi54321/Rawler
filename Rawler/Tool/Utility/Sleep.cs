using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    /// <summary>
    /// 指定した時間、プログラムを止めます。
    /// デフォルト値は３で3秒停止します。
    /// </summary>
    public class Sleep : RawlerBase
    {
        public override void Run(bool runChildren)
        {
            if (SleepTimeSpan != null)
            {
                System.Threading.Thread.Sleep(SleepTimeSpan);
            }
            else
            {
                System.Threading.Thread.Sleep((int)(sleepSeconds * 1000));
            }

            base.Run(runChildren);
        }

        public TimeSpan SleepTimeSpan { get; set; }

        private double sleepSeconds = 3;

        /// <summary>
        /// スリープする時間です。
        /// </summary>
        public double SleepSeconds {
            get { return sleepSeconds; }
            set { sleepSeconds = value; }
        }

        public override string Text
        {
            get
            {
                return GetText();
            }
        }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }


        /// <summary>
        /// クローンを作る。
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Sleep>(parent);
        }
    }
}
