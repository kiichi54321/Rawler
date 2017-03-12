using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace MyLib.Collections
{
    public class ListDictionary<Tkey, Tval> : Dictionary<Tkey, List<Tval>>
    {
        public ListDictionary()
            : base()
        {

        }


        /// <summary>
        /// 自動的にリストに追加します。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Add(Tkey key,Tval val)
        {
            List<Tval> list;
            if (this.TryGetValue(key, out list)== false)
            {
                list = new List<Tval>();
                this.Add(key, list);
            }
            list.Add(val);
                
        }

        public void AddRange(Tkey key, ICollection<Tval> collection)
        {
            List<Tval> list;
            if (this.TryGetValue(key, out list) == false)
            {
                list = new List<Tval>();
                this.Add(key, list);
            }
            list.AddRange(collection);

        }

        public void Remove(Tkey key, Tval val)
        {
            List<Tval> list;
            if (this.TryGetValue(key, out list))
            {
                list.Remove(val);
            }
            
        }

        public void Clear(Tkey key)
        {
            List<Tval> list;
            if (this.TryGetValue(key, out list))
            {
                list.Clear();
            }            
        }

        /// <summary>
        /// Dictionary の素の形式に戻す。
        /// </summary>
        /// <returns></returns>
        public Dictionary<Tkey, List<Tval>> ToDic()
        {
            Dictionary<Tkey, List<Tval>> dic = new Dictionary<Tkey, List<Tval>>();
            foreach (var item in this)
            {
                dic.Add(item.Key, item.Value);
            }
            return dic;
        }


    }
}
