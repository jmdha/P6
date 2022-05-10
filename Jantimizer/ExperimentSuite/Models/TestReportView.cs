using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestReportView
    {
        public string CaseName { get; set; }
        public ulong DatabasePredicted { get; set; }
        public ulong DatabaseActual { get; set; }
        public ulong EstimatorPredicted { get; set; }
        public decimal DatabaseAcc { get; }
        public decimal EstimatorAcc { get; }
        public ulong DatabaseOffBy { get; set; }
        public ulong EstimatorOffBy { get; set; }
        public bool IsBetter { get; }
        public bool OverEstimated { get; }
        public bool UnderEstimated { get; }
        public ulong AbstractStorageUsageBytes { get; }
        public ulong AbstractDatabaseSizeBytes { get; }
        public decimal AbstractStorageUsagePercent { get; }

        public TestReportView(TestReport report)
        {
            CaseName=report.CaseName;
            DatabasePredicted = report.DatabasePredicted;
            DatabaseActual= report.DatabaseActual;
            EstimatorPredicted= report.EstimatorPredicted;
            DatabaseAcc= report.DatabaseAcc;
            EstimatorAcc= report.EstimatorAcc;
            DatabaseOffBy = report.DatabaseOffBy;
            EstimatorOffBy= report.EstimatorOffBy;
            IsBetter= report.IsBetter;
            UnderEstimated= report.UnderEstimated;
            OverEstimated= report.OverEstimated;
            AbstractDatabaseSizeBytes= report.AbstractDatabaseSizeBytes;
            AbstractStorageUsageBytes= report.AbstractStorageUsageBytes;
            AbstractStorageUsagePercent = report.AbstractStorageUsagePercent;
        }
    }
}
