using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RawlerLib.Collections
{
    /// <summary>
    /// Keyの値によるソート可能なペア
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Pair<TKey, TValue> : IComparable<Pair<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        private TKey key;

        public TKey Key
        {
            get { return key; }
            set { key = value; }
        }
        private TValue value;

        public TValue Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public Pair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public int CompareTo(Pair<TKey, TValue> other)
        {
            return  this.Key.CompareTo(other.Key);
        }
    }
}
