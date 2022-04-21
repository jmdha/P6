using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.Dictionaries
{
    public class DualDictionary<T1, T2>
        where T1 : DualKey
        where T2 : notnull
    {
        private Dictionary<string, Dictionary<string, T2>> _dict;
        private List<T1> _refs;

        public DualDictionary()
        {
            _dict = new Dictionary<string, Dictionary<string, T2>>();
            _refs = new List<T1>();
        }

        public void Add(T1 key, T2 value)
        {
            if (_dict.ContainsKey(key.Key1))
            {
                if (_dict[key.Key1].ContainsKey(key.Key2))
                    throw new ArgumentException("Key pair already in dictionary!");
                _dict[key.Key1].Add(key.Key2, value);
            }
            else
            {
                _dict.Add(key.Key1, new Dictionary<string, T2>());
                _dict[key.Key1].Add(key.Key2, value);
            }
            _refs.Add(key);
        }

        public T2 this[T1 key] { get { return _dict[key.Key1][key.Key2]; } }
        public List<string> this[string firstKey] { get { return _dict[firstKey].Keys.ToList(); } }

        public bool DoesContain(T1 key)
        {
            return _dict.ContainsKey(key.Key1) && _dict[key.Key1].ContainsKey(key.Key2);
        }

        public bool DoesContain(string firstKey)
        {
            return _dict.ContainsKey(firstKey);
        }

        public List<T1> Keys => _refs;
    }
}
