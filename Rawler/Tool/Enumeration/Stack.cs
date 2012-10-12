using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public class UrlStack:RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        Stack<string> stack = new Stack<string>();

        TextVauleList urls = new TextVauleList();
        public TextVauleList Urls
        {
            get { return urls; }
            set 
            {
                urls = value;
            }
        }

        public string StartUrl { get; set; }

        public void Push(string text)
        {
            stack.Push(text);
        }

        public List<string> GetStackUrl()
        {
            return stack.ToList();
        }


        public override void Run(bool runChildren)
        {
            if (StartUrl != null)
            {
                this.Push(StartUrl);
            }
            foreach (var item in urls.Reverse<TextVaule>())
            {
                this.Push(item.Value);
            }

            OnPushUrlEvent();

            while (stack.Count > 0)
            {
                this.SetText(stack.Pop());
                base.Run(runChildren);
                OnPushUrlEvent();
            }
        }

        public event EventHandler<PushUrlEventArgs> PushUrl;

        protected void OnPushUrlEvent()
        {
            if (PushUrl != null)
            {
                PushUrl(this, new PushUrlEventArgs( this) );
            }
        }




        public class PushUrlEventArgs : EventArgs
        {
            public UrlStack UrlStack { get; set; }

            public PushUrlEventArgs(UrlStack stack)
            {
                this.UrlStack = stack;
            }
        }

    }

    public class PushUrl : RawlerBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        public override RawlerBase Clone(RawlerBase parent)
        {
            return base.Clone<PushUrl>(parent);
        }

        public override void Run(bool runChildren)
        {
            var stack = this.GetAncestorRawler().OfType<UrlStack>();
            if (stack.Any())
            {
                stack.First().Push(GetText());
            }
            else
            {
                ReportManage.ErrReport(this, "上流にUrlStackgaがありません");
            }
            base.Run(runChildren);
        }
    }
}
