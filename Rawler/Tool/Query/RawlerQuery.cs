using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Rawler.Tool
{
    [ContentProperty("Child")]
    public class RawlerQuery
    {
        public virtual IEnumerable<string> Query(IEnumerable<string> list)
        {
            return list;
        }

        public IEnumerable<string> RunQuery(IEnumerable<string> list)
        {
            IEnumerable<string> list2 = Query(list);
            if (Child != null)
            {
                list2 = Child.RunQuery(list2);
            }
            return list2;
        }

        public RawlerQuery Child { get; set; }

        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public virtual string ObjectName
        {
            get { return this.GetType().Name; }
        }
        

        public T Clone<T>()
            where T:RawlerQuery
        {
            var clone = (this as T).MemberwiseClone() as T;
            if (Child != null)
            {
                clone.Child = Child.Clone();
            }
            return clone;

        }


        public virtual RawlerQuery Clone()
        {
            return Clone<RawlerQuery>();
        }
    }
}
