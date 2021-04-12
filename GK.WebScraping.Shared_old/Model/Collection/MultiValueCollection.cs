using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.WebScraping.Shared.Model.Collection
{
    internal class MultiValueCollection<T1, T2>
    {
        private Dictionary<T1, List<T2>> _innerData;

        public Int32 Count { get { return this._innerData.Count; } }
        public MultiValueCollection()
        {
            this._innerData = new Dictionary<T1, List<T2>>();
        }

        public void Add(T1 key, T2 value)
        {
            if (this._innerData.ContainsKey(key))
                this._innerData[key].Add(value);
            else
            {
                this._innerData.Add(key, new List<T2>());
                this._innerData[key].Add(value);

            }

        }
        public T2 this[T1 key, Int32 i]
        {
            get
            {
                if (this.TryGetValue(key, i, out T2 retval))
                    return retval;
                return default(T2);
            }
            set
            {
                this._innerData[key][i] = value;
            }
        }

        public Boolean TryGetValue(T1 key, Int32 i, out T2 value)
        {
            value = default(T2);
            if (this._innerData.ContainsKey(key) && this._innerData[key].Count >= (i + 1))
            {
                value = this._innerData[key][i];
                return true;
            }
            else
                return false;
        }

        internal bool Exists(T1 key, Int32 i)
        {
            if (this._innerData.ContainsKey(key))
            {
                return this._innerData[key].Count - 1 <= i;
            }

            return false;
        }

    }
}
