using Histograms.Caches;
using QueryPlanParser.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;

namespace ExperimentSuite.Controllers
{
    internal class CacheController
    {
        public delegate void ClearViewPanelHandler();
        public event ClearViewPanelHandler? ClearViewPanel;

        public CacheController()
        {
        }

        public void ClearLocalCaches()
        {
            if (QueryPlanCacher.Instance != null)
                QueryPlanCacher.Instance.ClearCache();
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.ClearCache();

            ClearViewPanel?.Invoke();
        }

        public void ClearFileAndLocalCaches()
        {
            if (QueryPlanCacher.Instance != null)
                QueryPlanCacher.Instance.ClearCache(true);
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.ClearCache(true);

            ClearViewPanel?.Invoke();
        }

        public List<CacheItem> GetAllCacheItems()
        {
            var cacheItems = new List<CacheItem>();
            if (HistogramCacher.Instance != null)
                cacheItems.AddRange(HistogramCacher.Instance.GetAllCacheItems());
            if (QueryPlanCacher.Instance != null)
                cacheItems.AddRange(QueryPlanCacher.Instance.GetAllCacheItems());
            return cacheItems;
        }

        public void LoadCachesFromFile()
        {
            if (QueryPlanCacher.Instance != null)
                QueryPlanCacher.Instance.LoadCacheFromFile();
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.LoadCacheFromFile();
        }
    }
}
