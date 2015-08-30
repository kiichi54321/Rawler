using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace Rawler.Tool.Date
{
    public class DateTime:RawlerBase
    {
        protected System.DateTime dateTime = new System.DateTime();

        //public int Year { get; set; }
        //public int Month { get; set; }
        //public int Day { get; set; }

        //public int Hour { get; set; }
        //public int Minitu { get; set; }

        DateTimeStringType stringType = DateTimeStringType.ToString;

        public DateTimeStringType StringType
        {
            get { return stringType; }
            set { stringType = value; }
        }

        public override void Run(bool runChildren)
        {
            SetText(GetDateString(dateTime));
            
            base.Run(runChildren);
        }

        public string GetDateString(System.DateTime date)
        {
            if(stringType == DateTimeStringType.ToString)
            {
                return date.ToString();
            }
            else if(stringType == DateTimeStringType.ToLongDateString)
            {
                return date.ToLongDateString();
            }
            else if(stringType == DateTimeStringType.ToLongTimeString)
            {
                return date.ToLongTimeString();
            }
            else if( stringType == DateTimeStringType.ToShortDateString)
            {
                return date.ToShortDateString();
            }
            else if(stringType == DateTimeStringType.ToShortTimeString)
            {
                return date.ToShortTimeString();
            }
            return date.ToString();

        }

    }

    /// <summary>
    /// DateTimeをStringに変換する方法です。
    /// </summary>
    public enum DateTimeStringType
    {
        /// <summary>
        /// 
        /// </summary>
        ToLongDateString,
        /// <summary>
        /// 
        /// </summary>
        ToLongTimeString, 
        /// <summary>
        /// 
        /// </summary>
        ToShortDateString, 
        /// <summary>
        /// 
        /// </summary>
        ToShortTimeString,
        /// <summary>
        /// 
        /// </summary>
        ToString

    }


    public class Today:DateTime
    {
        public Today()
        {
            this.dateTime = System.DateTime.Today;
        }
    }

    public class Now:DateTime
    {
        public Now()
        {
            this.dateTime = System.DateTime.Now;
        }
    }
}
