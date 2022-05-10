using DatabaseConnector;
using DatabaseConnector.Connectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Milestoner;
using Milestoner.DataGatherers;
using Milestoner.DepthCalculators;
using Milestoner.Milestoners;
using QueryEstimator;
using QueryEstimator.Models;
using QueryPlanParser;
using QueryPlanParser.Models;
using QueryPlanParser.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Models.JsonModels;
using Tools.Services;

namespace JantimizerSystemTests
{
    [TestClass]
    public class SystemTests
    {
        private static string _casePath = "../../../Cases/";
        private static string _setupPath = "../../../Setups/";
        internal static SecretsService<SystemTests> secrets = new SecretsService<SystemTests>();

        // We should expect 100% accuracte results with single joins
        [TestMethod]
        [DataRow("B_J1-G1_F0_a.json", "constant.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F0_b.json", "constant.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F1-L1.json", "constant.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_a.json", "constant.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_b.json", "constant.setup.posgresql.sql")]
        public async Task Constant_MinDepth_Constant_Tests(string caseFileName, string setupFileName)
        {
            // ARRANGE
            var queryFile = new JsonQuery(File.ReadAllText($"{_casePath}{caseFileName}"));
            var setupFileText = File.ReadAllText($"{_setupPath}{setupFileName}");

            if (!secrets.HasKey("POSGRESQL"))
                Assert.Inconclusive("User secrets not set! Ignore this if its run from github actions.");

            var properties = new ConnectionProperties(
                secrets.GetSecretsItem("POSGRESQL"),
                "postgres",
                "systemtests_constant");
            IDbConnector connector = new PostgreSqlConnector(properties);
            IQueryEstimator<JsonQuery> estimator = await GetEstimator(connector, setupFileText);
            AnalysisResult analysisResult = await GetActualResult(connector, queryFile.EquivalentSQLQuery);

            // ACT
            EstimatorResult jantimatorResult = estimator.GetQueryEstimation(queryFile);

            // ASSERT
            Assert.AreEqual(analysisResult.ActualCardinality, jantimatorResult.EstimatedCardinality);
        }

        // We should expect 100% accuracte results with single joins
        [TestMethod]
        [DataRow("B_J1-G1_F0_a.json", "random.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F0_b.json", "random.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F1-L1.json", "random.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_a.json", "random.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_b.json", "random.setup.posgresql.sql")]
        public async Task Random_MinDepth_Constant_Tests(string caseFileName, string setupFileName)
        {
            // ARRANGE
            var queryFile = new JsonQuery(File.ReadAllText($"{_casePath}{caseFileName}"));
            var setupFileText = File.ReadAllText($"{_setupPath}{setupFileName}");

            if (!secrets.HasKey("POSGRESQL"))
                Assert.Inconclusive("User secrets not set! Ignore this if its run from github actions.");

            var properties = new ConnectionProperties(
                secrets.GetSecretsItem("POSGRESQL"),
                "postgres",
                "systemtests_random");
            IDbConnector connector = new PostgreSqlConnector(properties);
            IQueryEstimator<JsonQuery> estimator = await GetEstimator(connector, setupFileText);
            AnalysisResult analysisResult = await GetActualResult(connector, queryFile.EquivalentSQLQuery);

            // ACT
            EstimatorResult jantimatorResult = estimator.GetQueryEstimation(queryFile);

            // ASSERT
            Assert.AreEqual(analysisResult.ActualCardinality, jantimatorResult.EstimatedCardinality);
        }

        // We should expect 100% accuracte results with single joins
        [TestMethod]
        [DataRow("B_J1-G1_F0_a.json", "clumped.possible.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F0_b.json", "clumped.possible.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F1-L1.json", "clumped.possible.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_a.json", "clumped.possible.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_b.json", "clumped.possible.setup.posgresql.sql")]
        public async Task Clumped_Possible_MinDepth_Constant_Tests(string caseFileName, string setupFileName)
        {
            // ARRANGE
            var queryFile = new JsonQuery(File.ReadAllText($"{_casePath}{caseFileName}"));
            var setupFileText = File.ReadAllText($"{_setupPath}{setupFileName}");

            if (!secrets.HasKey("POSGRESQL"))
                Assert.Inconclusive("User secrets not set! Ignore this if its run from github actions.");

            var properties = new ConnectionProperties(
                secrets.GetSecretsItem("POSGRESQL"),
                "postgres",
                "systemtests_clumped_possible");
            IDbConnector connector = new PostgreSqlConnector(properties);
            IQueryEstimator<JsonQuery> estimator = await GetEstimator(connector, setupFileText);
            AnalysisResult analysisResult = await GetActualResult(connector, queryFile.EquivalentSQLQuery);

            // ACT
            EstimatorResult jantimatorResult = estimator.GetQueryEstimation(queryFile);

            // ASSERT
            Assert.AreEqual(analysisResult.ActualCardinality, jantimatorResult.EstimatedCardinality);
        }

        // We should expect 100% accuracte results with single joins
        [TestMethod]
        [DataRow("B_J1-G1_F0_a.json", "clumped.difficult.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F0_b.json", "clumped.difficult.setup.posgresql.sql")]
        [DataRow("B_J1-G1_F1-L1.json", "clumped.difficult.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_a.json", "clumped.difficult.setup.posgresql.sql")]
        [DataRow("B_J1-L1_F0_b.json", "clumped.difficult.setup.posgresql.sql")]
        public async Task Clumped_Difficult_MinDepth_Constant_Tests(string caseFileName, string setupFileName)
        {
            // ARRANGE
            var queryFile = new JsonQuery(File.ReadAllText($"{_casePath}{caseFileName}"));
            var setupFileText = File.ReadAllText($"{_setupPath}{setupFileName}");

            if (!secrets.HasKey("POSGRESQL"))
                Assert.Inconclusive("User secrets not set! Ignore this if its run from github actions.");

            var properties = new ConnectionProperties(
                secrets.GetSecretsItem("POSGRESQL"),
                "postgres",
                "systemtests_clumped_difficult");
            IDbConnector connector = new PostgreSqlConnector(properties);
            IQueryEstimator<JsonQuery> estimator = await GetEstimator(connector, setupFileText);
            AnalysisResult analysisResult = await GetActualResult(connector, queryFile.EquivalentSQLQuery);

            // ACT
            EstimatorResult jantimatorResult = estimator.GetQueryEstimation(queryFile);

            // ASSERT
            Assert.AreEqual(analysisResult.ActualCardinality, jantimatorResult.EstimatedCardinality);
        }

        #region Private Test Methods

        private static bool? haveChecked;
        private async Task<bool> IsPostgresRunning(IDbConnector connector)
        {
            if (haveChecked != null)
                return (bool)haveChecked;
            haveChecked = await connector.CheckConnectionAsync();
            return (bool)haveChecked;
        }

        private async Task GenerateMilestones(IMilestoner milestoner)
        {
            milestoner.ClearMilestones();

            List<Func<Task>> milestoneTasks = await milestoner.AddMilestonesFromDBTasks();

            List<Task> results = new List<Task>();
            foreach (Func<Task> funcs in milestoneTasks)
            {
                results.Add(funcs.Invoke());
            }
            while (results.Any())
            {
                var finishedTask = await Task.WhenAny(results);
                results.Remove(finishedTask);
                await finishedTask;
            }
        }

        private async Task BindMilestones(IMilestoner milestoner)
        {
            List<Func<Task>> compareTasks = milestoner.CompareMilestonesWithDBDataTasks();

            List<Task> results = new List<Task>();
            foreach (Func<Task> funcs in compareTasks)
            {
                results.Add(funcs.Invoke());
            }
            while (results.Any())
            {
                var finishedTask = await Task.WhenAny(results);
                results.Remove(finishedTask);
                await finishedTask;
            }
        }

        private async Task<IQueryEstimator<JsonQuery>> GetEstimator(IDbConnector connector, string setupFileText)
        {
            IDataGatherer dataGatherer = new PostgresDataGatherer(connector.ConnectionProperties);
            IDepthCalculator depthCalculator = new ConstantDepth(1);
            IMilestoner milestoner = new MinDepthMilestoner(dataGatherer, depthCalculator);
            IQueryEstimator<JsonQuery> estimator = new JsonQueryEstimator(milestoner, 10);

            if (!await IsPostgresRunning(connector))
                Assert.Inconclusive("Postgres is not running! Ignore this if its run from github actions.");

            await connector.CallQueryAsync(setupFileText);

            await GenerateMilestones(milestoner);
            await BindMilestones(milestoner);

            return estimator;
        }

        private async Task<AnalysisResult> GetActualResult(IDbConnector connector, string query)
        {
            IPlanParser parser = new PostgreSqlParser();
            DataSet dbResult = await connector.AnalyseExplainQueryAsync(query);
            AnalysisResult analysisResult = parser.ParsePlan(dbResult);

            return analysisResult;
        }


        #endregion
    }
}