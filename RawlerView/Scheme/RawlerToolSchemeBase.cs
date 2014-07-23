<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace RawlerView.Scheme
{
    [ContentProperty("Children")]
    [RuntimeNameProperty("Name")]
    public class RawlerToolSchemeBase
    {
        public string Name { get; set; }

        RawlerToolSchemeBaseCollection children = new RawlerToolSchemeBaseCollection();
        public RawlerToolSchemeBaseCollection Children
        {
            get { return children; }
        }
    }

    /// <summary>
    /// 子の集合クラス
    /// </summary>
    [ContentProperty("Items")]
    [Serializable]
    public class RawlerToolSchemeBaseCollection : System.Collections.ObjectModel.ObservableCollection<RawlerToolSchemeBase>
    {

    }

}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace RawlerView.Scheme
{
    [ContentProperty("Children")]
    [RuntimeNameProperty("Name")]
    public class RawlerToolSchemeBase
    {
        public string Name { get; set; }

        RawlerToolSchemeBaseCollection children = new RawlerToolSchemeBaseCollection();
        public RawlerToolSchemeBaseCollection Children
        {
            get { return children; }
        }
    }

    /// <summary>
    /// 子の集合クラス
    /// </summary>
    [ContentProperty("Items")]
    [Serializable]
    public class RawlerToolSchemeBaseCollection : System.Collections.ObjectModel.ObservableCollection<RawlerToolSchemeBase>
    {

    }

}
>>>>>>> 6ddbdbbc3a7813a9636b3cdd2ae14b324c102419
