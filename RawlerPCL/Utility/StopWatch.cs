using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rawler
{
    public class StopWatch:RawlerBase
    {
        public string ReportText { get; set; }
        bool viewTotal = false;

        public bool ViewTotal
        {
            get { return viewTotal; }
            set { viewTotal = value; }
        }
        bool viewParent = true;

        public bool ViewParent
        {
            get { return viewParent; }
            set { viewParent = value; }
        }

  
        public override void Run(bool runChildren)
        {
            Write(this,ReportText,ViewTotal);
            base.Run(runChildren);
        }

        static Stopwatch sw = new Stopwatch();
        static long total = 0;

        static long Total
        {
            get { return StopWatch.total; }
            set { StopWatch.total = value; }
        }
        public static void Clear()
        {
            total = 0;
            sw.Restart();
        }

        public static void WriteConsole(string text)
        {
            var t = sw.ElapsedMilliseconds + "\t" + text;
            total += sw.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine(t);
            sw.Restart();
        }

        public void Write(RawlerBase rawler, string text, bool viewTotal)
        {
            if (DoRun)
            {
                if (viewParent)
                {
                    text = this.Parent.ToObjectString() + " " + text;
                }

                var t = sw.ElapsedMilliseconds + "\t" + text;
                total += sw.ElapsedMilliseconds;
                if (viewTotal)
                {
                    ReportManage.Report(rawler, t,true,true);
                    ReportManage.Report(rawler,"Total:"+ total, true, true);
                }
                else
                {
                    ReportManage.Report(rawler, t,true,true);
                }
                sw.Restart();
            }
        }

        static bool doRun = true;

        public static bool DoRun
        {
            get { return StopWatch.doRun; }
            set { StopWatch.doRun = value; }
        }

    }

    public class StopWatchStop:RawlerBase
    {
        public override void Run(bool runChildren)
        {
            StopWatch.Clear();
            base.Run(runChildren);
        }
    }
}
