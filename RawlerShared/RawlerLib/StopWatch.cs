using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawlerLib.Timer
{
    /// <summary>
    /// パフォーマンス計測
    /// </summary>
    public static class StopWatch
    {
        static Stopwatch sw = new Stopwatch();
        static long total = 0;
        /// <summary>
        /// Total時間（Milliseconds）
        /// </summary>
        public static long Total
        {
            get { return StopWatch.total; }
            set { StopWatch.total = value; }
        }
        /// <summary>
        /// Total時間を消す
        /// </summary>
        public static void Clear()
        {
            total = 0;
            sw.Restart();
        }

        /// <summary>
        /// Console.WriteLine する
        /// </summary>
        /// <param name="text"></param>
        public static void Write(string text)
        {
            if (DoRun)
            {
                var t = sw.ElapsedMilliseconds + "\t" + text;
                total += sw.ElapsedMilliseconds;
                System.Diagnostics.Debug.WriteLine(t);
                if (WriteEvent != null) WriteEvent(t);
                sw.Restart();
            }
        }

        static bool doRun = true;

        /// <summary>
        /// 時間計測を実行
        /// </summary>
        public static bool DoRun
        {
            get { return StopWatch.doRun; }
            set { StopWatch.doRun = value; }
        }

        /// <summary>
        /// Writeの後に起こす処理
        /// </summary>
        public static Action<string> WriteEvent;
    }
}
