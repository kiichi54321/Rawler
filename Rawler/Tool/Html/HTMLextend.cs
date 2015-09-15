using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;
using Rawler.Tool.Html.Core;

namespace Rawler.Tool.Html.Core
{
    public class Tags2 : Tags
    {
        public Tags2(string tag)
        {
            base.Tag = tag;            
        }

        protected string Tag { get; set; }
        protected string IdName { get; set; }
        protected string ClassName { get; set; }

        public string id
        {
            get { return base.IdName; }
            set { base.IdName = value; }
        }

        public string @class
        {
            get { return base.ClassName; }
            set { base.ClassName = value; }
        }
    }

}

/// <summary>
/// htmlのタグと同じ名前にしたTags
/// </summary>
namespace Rawler.Tool.Html
{


    /// <summary>
    /// Divタグ
    /// </summary>
    public class div :Tags2
    {
        public div():base("div")
        {
        }
    }

    public class h1 : Tags2
    {
        public h1():base("h1")
        {
        }
    }

    public class h2 : Tags2
    {
        public h2() : base("h2")
        {
        }
    }

    public class h3 : Tags2
    {
        public h3() : base("h3")
        {
        }
    }

    public class h4 : Tags2
    {
        public h4() : base("h4")
        {
        }
    }

    public class h5 : Tags2
    {
        public h5() : base("h5")
        {
        }
    }

    public class h6 : Tags2
    {
        public h6() : base("h6")
        {
        }
    }

    public class table : Tags2
    {
        public table() : base("table")
        {
        }
    }

    public class tbody : Tags2
    {
        public tbody() : base("tbody")
        {
        }
    }
    public class thead : Tags2
    {
        public thead() : base("thead")
        {
        }
    }
    
    public class tfoot : Tags2
    {
        public tfoot() : base("tfoot")
        {
        }
    }


    public class tr : Tags2
    {
        public tr() : base("tr")
        {
        }
    }
    public class th : Tags2
    {
        public th() : base("th")
        {
        }
    }

    public class td : Tags2
    {
        public td() : base("td")
        {
        }
    }
    public class p : Tags2
    {
        public p() : base("p")
        {
        }
    }

    public class form:Tags2
    {
        public form():base("form")
        { }
    }

    public class span : Tags2
    {
        public span() : base("span")
        {
        }
    }

    public class ul : Tags2
    {
        public ul() : base("ul")
        {
        }
    }

    public class li : Tags2
    {
        public li() : base("li")
        {
        }
    }

    public class ol : Tags2
    {
        public ol() : base("ol")
        {
        }
    }


    public class  article: Tags2
    {
        public article() : base("article")
        {
        }
    }

    public class section : Tags2
    {
        public section() : base("section")
        {
        }
    }

    public class head : Tags2
    {
        public head() : base("head")
        {
        }
    }

    public class title : Tags2
    {
        public title() : base("title")
        {
        }
    }
    
    public class address : Tags2
    {
        public address() : base("address")
        {
        }
    }

    public class caption : Tags2
    {
        public caption() : base("caption")
        {
        }
    }

}
