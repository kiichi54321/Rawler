using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RawlerView
{
    public static class ViewTask
    {
        public static TaskScheduler UISyncContext { get; set; }
        public static void UITask(Action action)
        {
            Task reportProgressTask = Task.Factory.StartNew(() =>
            {
                action();
            },
         CancellationToken.None,
         TaskCreationOptions.None,
         ViewTask.UISyncContext);
            reportProgressTask.Wait();
        }
    }
}
