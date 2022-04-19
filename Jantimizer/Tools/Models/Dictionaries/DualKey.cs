using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.Dictionaries
{
    public abstract class DualKey
    {
        public string Key1 { get; set; }
        public string Key2 { get; set; }

        protected DualKey(string key1, string key2)
        {
            Key1 = key1;
            Key2 = key2;
        }

        public override bool Equals(object? obj)
        {
            return obj is DualKey key &&
                   Key1 == key.Key1 &&
                   Key2 == key.Key2;
        }

        public override int GetHashCode()
        {
            return Key1.GetHashCode() + Key2.GetHashCode();
        }
    }
}
