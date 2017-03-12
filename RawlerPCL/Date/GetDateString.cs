using Rawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Date
{
    public class GetDateString:RawlerMultiBase
    {
        DateType dateType = DateType.LongDate;

        public DateType DateType
        {
            get { return dateType; }
            set { dateType = value; }
        }


        public override void Run(bool runChildren)
        {
            List<string> list = new List<string>();
            if(DateType == DateType.LongDate)
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"\d{4}/\d{1,2}/\d{1,2} \d{2}:\d{2}");

                foreach(System.Text.RegularExpressions.Match item in reg.Matches(GetText()))
                {
                    list.Add(item.Value);
                }
            }
            else if(DateType ==DateType.ShortDate)
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"\d{4}/\d{1,2}/\d{1,2}");

                foreach (System.Text.RegularExpressions.Match item in reg.Matches(GetText()))
                {
                    list.Add(item.Value);
                }
            }
            RunChildrenForArray(runChildren, list);

          
        }
    }

    public enum DateType
    {
        LongDate,ShortDate
    }
}
