using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestReportView
    {
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

        public TestReportView(ulong databasePredicted, ulong databaseActual, ulong estimatorPredicted, decimal databaseAcc, decimal estimatorAcc, ulong databaseOffBy, ulong estimatorOffBy, bool isBetter, bool overEstimated, bool underEstimated, ulong abstractStorageUsageBytes, ulong abstractDatabaseSizeBytes)
        {
            DatabasePredicted = databasePredicted;
            DatabaseActual = databaseActual;
            EstimatorPredicted = estimatorPredicted;
            DatabaseAcc = databaseAcc;
            EstimatorAcc = estimatorAcc;
            DatabaseOffBy = databaseOffBy;
            EstimatorOffBy = estimatorOffBy;
            IsBetter = isBetter;
            OverEstimated = overEstimated;
            UnderEstimated = underEstimated;
            AbstractStorageUsageBytes = abstractStorageUsageBytes;
            AbstractDatabaseSizeBytes = abstractDatabaseSizeBytes;
        }

        public TestReportView(TestReport report)
        {
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
        }
    }
}
