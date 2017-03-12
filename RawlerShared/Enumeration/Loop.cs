using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Core;

namespace Rawler
{
    public class Loop : RawlerBase
    {
        #region テンプレ
        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<Loop>(parent);
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
        public async override void Run(bool runChildren)
        {
            while (isBreaked == false)
            {
                base.Run(runChildren);
                int time = Math.Max((sleepTime + (int)(sleepWide * (randam.NextDouble() - 0.5) * 2)) * 1000, 1000);
                await System.Threading.Tasks.Task.Delay(time);
            }    
        }


        Random randam = new Random();
        int sleepTime = 3;

        int sleepWide = 0;

        public int SleepWide
        {
            get { return sleepWide; }
            set { sleepWide = value; }
        }

        public int SleepTime
        {
            get { return sleepTime; }
            set { sleepTime = value; }
        }

        bool isBreaked = false;

        public void Break()
        {
            isBreaked = true;
        }


        /// <summary>
        /// 子が参照するテキスト。
        /// </summary>
        public override string Text
        {
            get
            {
                return GetText();
            }
        }


    }

    /// <summary>
    /// Loopに対して終了命令を出す。
    /// </summary>
    public class LoopEnd:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            var loop = this.GetUpperRawler<Loop>();
            if(loop !=null)
            {
                loop.Break();
            }
            else
            {
                ReportManage.ErrUpperNotFound<Loop>(this);
            }
            base.Run(runChildren);
        }
    }
}
