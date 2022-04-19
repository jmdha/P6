using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models
{
    public class MultiDictionary<T1, T2, T3>
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
    {
        private Dictionary<T1, Dictionary<T2, T3>> _dict;

        public MultiDictionary()
        {
            _dict = new Dictionary<T1, Dictionary<T2, T3>>();
        }

        public void Add(T1 key1, T2 key2, T3 value)
        {
            if (_dict.ContainsKey(key1))
            {
                if (_dict[key1].ContainsKey(key2))
                    throw new ArgumentException("Key pair already in dictionary!");
                _dict[key1].Add(key2, value);
            }
            else
            {
                _dict.Add(key1, new Dictionary<T2, T3>());
                _dict[key1].Add(key2, value);
            }
        }

        public T3 this[T1 key1, T2 key2] { get { return _dict[key1][key2]; } }
        public List<T2> this[T1 key1] { get { return _dict[key1].Keys.ToList(); } }

        public bool DoesContain(T1 key1, T2 key2)
        {
            return _dict.ContainsKey(key1) && _dict[key1].ContainsKey(key2);
        }

        public bool DoesContain(T1 key1)
        {
            return _dict.ContainsKey(key1);
        }

        public List<T1> Keys => _dict.Keys.ToList();
    }
}
