using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawler.Tool
{
    public static class ViewManage
    {
        static Dictionary<string, List<string>> viewData = new Dictionary<string, List<string>>();

        public static Dictionary<string, List<string>> Data
        {
            get { return ViewManage.viewData; }
            set { ViewManage.viewData = value; }
        }

        public static void Add(string name, List<string> data)
        {
            if (viewData.ContainsKey(name))
            {
                viewData[name] = data;
            }
            else
            {
                viewData.Add(name, data);
            }
        }

        public static List<string> Keys
        {
            get
            {
                return new List<string>( viewData.Keys);
            }
        }

        public static List<string> GetValue(string key)
        {
            if (viewData.ContainsKey(key))
            {
                return viewData[key];
            }
            else
            {
                return new List<string>();
            }
        }

        public static void Clear()
        {
            viewData.Clear();
        }

    }
}
