using System;
using System.Collections.Generic;

namespace GK.WebScraping.Model.Code.Collections
{
    public class Dictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>, IDictionary<Tuple<TKey1, TKey2>, TValue>
    {

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return base[Tuple.Create(key1, key2)]; }
            set { base[Tuple.Create(key1, key2)] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            base.Add(Tuple.Create(key1, key2), value);
        }

        public new bool ContainsValue(TValue value)
        {
            return base.ContainsValue(value);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return base.ContainsKey(Tuple.Create(key1, key2));
        }
        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
        {
            return this.TryGetValue(Tuple.Create(key1, key2), out value);
        }
    }
    public class Dictionary<TKey1, TKey2, TKey3, TValue> : Dictionary<Tuple<TKey1, TKey2, TKey3>, TValue>, IDictionary<Tuple<TKey1, TKey2, TKey3>, TValue>
    {

        public TValue this[TKey1 key1, TKey2 key2, TKey3 key3]
        {
            get { return base[Tuple.Create(key1, key2, key3)]; }
            set { base[Tuple.Create(key1, key2, key3)] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TKey3 key3, TValue value)
        {
            base.Add(Tuple.Create(key1, key2, key3), value);
        }

        public new bool ContainsValue(TValue value)
        {
            return base.ContainsValue(value);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return base.ContainsKey(Tuple.Create(key1, key2, key3));
        }

        public bool TryGetValue(TKey1 key1, TKey2 key2, TKey3 key3, out TValue value)
        {
            return this.TryGetValue(Tuple.Create(key1, key2, key3), out value);
        }

    }
}
