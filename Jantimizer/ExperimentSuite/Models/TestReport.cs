using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestReport
    {
        public string ExperimentName { get; set; }
        public string Category { get; set; }
        public string CaseName { get; set; }
        public string DatabaseName { get; set; }
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
        internal EstimatorResult? EstimatorResult;

        public TestReport()
        {
            ExperimentName = "";
            Category = "";
            CaseName = "";
            DatabaseName = "";
            DatabasePredicted = 0;
            DatabaseActual = 0;
            EstimatorPredicted = 0;
            DatabaseAcc = 0;
            EstimatorAcc = 0;
            EstimatorResult = null;
        }

        public TestReport(string experimentName, string category, string caseName, string databaseName, ulong databasePredicted, ulong databaseActual, ulong estimatorPredicted, ulong abstractStorageUsageBytes, ulong abstractDatabaseSizeBytes, EstimatorResult result)
        {
            ExperimentName = experimentName;
            Category = category;
            CaseName = caseName;
            DatabaseName = databaseName;
            DatabasePredicted = databasePredicted;
            DatabaseActual = databaseActual;
            EstimatorPredicted = estimatorPredicted;
            AbstractStorageUsageBytes = abstractStorageUsageBytes;
            AbstractDatabaseSizeBytes = abstractDatabaseSizeBytes;
            DatabaseAcc = GetAccuracy(databaseActual, databasePredicted);
            EstimatorAcc = GetAccuracy(databaseActual, estimatorPredicted);
            DatabaseOffBy = (ulong)Math.Abs((decimal)databaseActual - (decimal)databasePredicted);
            EstimatorOffBy = (ulong)Math.Abs((decimal)databaseActual - (decimal)estimatorPredicted);

            IsBetter = EstimatorOffBy < DatabaseOffBy;
            OverEstimated = estimatorPredicted > databaseActual;
            UnderEstimated = estimatorPredicted < databaseActual;

            EstimatorResult = result;
        }

        private decimal GetAccuracy(ulong actualValue, ulong predictedValue)
        {
            if (actualValue == 0 && predictedValue == 0)
                return 100;
            if (actualValue == 0)
                return 0;
            if (actualValue != 0 && predictedValue == 0)
                return 0;
            if (actualValue < predictedValue)
            {
                decimal value = ((decimal)actualValue / (decimal)predictedValue) * 100;
                return Math.Round(value, 2);
            }
            if (actualValue > predictedValue)
            {
                decimal value = ((decimal)predictedValue / (decimal)actualValue) * 100;
                return Math.Round(value, 2);
            }
            return 100;
        }
    }
}
