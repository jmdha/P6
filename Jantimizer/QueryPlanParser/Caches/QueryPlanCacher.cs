﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;
using System.Text.Json;

namespace QueryPlanParser.Caches
{
    public class QueryPlanCacher : BaseCacherService<ulong?>
    {
        public override FileInfo CacheFile { get; set; }
        public static QueryPlanCacher? Instance { get; set; }

        internal Dictionary<string, ulong> ActualCardinalityCache { get; set; }

        public QueryPlanCacher(string cacheFilePath)
        {
            ActualCardinalityCache= new Dictionary<string, ulong>();
            CacheFile = new FileInfo(cacheFilePath);
            if (CacheFile.Exists && UseCacheFile)
                LoadCacheFromFile();
            Instance = this;
        }

        public QueryPlanCacher()
        {
            ActualCardinalityCache = new Dictionary<string, ulong>();
            CacheFile = new FileInfo("none");
            UseCacheFile = false;
            Instance = this;
        }

        public override void AddToCacheIfNotThere(string hashValue, ulong? value)
        {
            if (!ActualCardinalityCache.ContainsKey(hashValue))
            {
                if (value != null)
                {
                    ActualCardinalityCache.Add(hashValue, (ulong)value);
                    if (UseCacheFile)
                        SaveCacheToFile();
                }
            }
        }

        public override ulong? GetValueOrNull(string hashValue)
        {
            if (ActualCardinalityCache.ContainsKey(hashValue))
                return ActualCardinalityCache[hashValue];
            return null;
        }

        public override void ClearCache(bool deleteFile = false)
        {
            if (CacheFile.Exists && deleteFile)
                CacheFile.Delete();
            ActualCardinalityCache.Clear();
        }

        public override void LoadCacheFromFile()
        {
            if (CacheFile.Exists)
            {
                ClearCache();
                var obj = JsonSerializer.Deserialize<Dictionary<string, ulong>>(File.ReadAllText(CacheFile.FullName));
                if (obj != null)
                    ActualCardinalityCache = obj;
            }
        }

        public override void SaveCacheToFile()
        {
            string jsonText = JsonSerializer.Serialize(ActualCardinalityCache);
            if (CacheFile.Exists)
                CacheFile.Delete();

            using (FileStream fs = CacheFile.Create())
            {
                byte[] info = new UTF8Encoding(true).GetBytes(jsonText);
                fs.Write(info, 0, info.Length);
            }
        }

        public override List<CacheItem> GetAllCacheItems()
        {
            var returnList = new List<CacheItem>();
            foreach (var key in ActualCardinalityCache.Keys)
                returnList.Add(new CacheItem(key, ActualCardinalityCache[key].ToString(), nameof(QueryPlanCacher)));
            return returnList;
        }
    }
}
