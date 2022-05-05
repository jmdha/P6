using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Tools.Helpers
{
    public static class DictionaryHelper
    {
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
        {
            if(dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public static void AddOrUpdateList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key, TValue value) where TKey : notnull
        {
            if (dictionary.ContainsKey(key))
                dictionary[key].Add(value);
            else
                dictionary.Add(key, new List<TValue>() { value });
        }

        public static TValue? GetIfThereOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return default;
        }

        public static List<TValue> GetIfThereOrDefaultList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key) where TKey : notnull
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return new List<TValue>();
        }
    }
}
