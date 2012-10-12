using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    class Write:IRawler
    {
        public IRawler Parent
        {
            get;
            set;
        }

        List<IRawler> children = new List<IRawler>();
        public List<IRawler> Children
        {
            get
            {
                return children;
            }

        }

        public void AddChildren(IRawler rawler)
        {
            rawler.Parent = this;
            children.Add(rawler);
        }

        public string Text
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.Text;
                }
                return string.Empty;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public void Run(bool runChildren)
        {
            throw new NotImplementedException();
        }



    }
}
