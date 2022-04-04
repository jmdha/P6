﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestReport
    {
        public string Category { get; set; }
        public string CaseName { get; set; }
        public string DatabaseName { get; set; }
        public ulong DatabasePredicted { get; set; }
        public ulong DatabaseActual { get; set; }
        public ulong OptimiserPredicted { get; set; }
        public decimal DatabaseAcc { get; }
        public decimal OptimiserAcc { get; }

        public TestReport(string category, string caseName, string databaseName, ulong databasePredicted, ulong databaseActual, ulong optimiserPredicted)
        {
            Category = category;
            CaseName = caseName;
            DatabaseName = databaseName;
            DatabasePredicted = databasePredicted;
            DatabaseActual = databaseActual;
            OptimiserPredicted = optimiserPredicted;
            DatabaseAcc = GetAccuracy(databaseActual, databasePredicted);
            OptimiserAcc = GetAccuracy(databaseActual, optimiserPredicted);
        }

        private decimal GetAccuracy(ulong actualValue, ulong predictedValue)
        {
            if (actualValue == 0 && predictedValue == 0)
                return 100;
            if (actualValue == 0)
                return -1;
            if (actualValue != 0 && predictedValue == 0)
                return -1;
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