using DatabaseConnector.Connectors;
using ExperimentSuite.Models;
using QueryEstimator;
using QueryPlanParser.Parsers;
using Segmentator.DataGatherers;
using Segmentator.DepthCalculators;
using Segmentator.Milestoners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models;
using Tools.Services;

namespace ExperimentSuite.SuiteDatas
{
    internal static class SuiteDataSets
    {
        internal static SecretsService<MainWindow> secrets = new SecretsService<MainWindow>();

        #region MySQL

        public static SuiteData GetMySQLSD_EquiDepth(JsonObject optionalTestSettings)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new MyConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new EquiDepthMilestoner(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                new ConstantDepth(JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize")));
            var mySQLEstimator = new JsonQueryEstimator(mySQLHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "EquiDepth",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLEstimator);
            return mySQLModel;
        }
        public static SuiteData GetMySQLSD_EquiDepth_DynamicDepth(JsonObject optionalTestSettings)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new MyConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new EquiDepthMilestoner(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                new DynamicDepth(useUniqueValueCount: false));
            var mySQLEstimator = new JsonQueryEstimator(mySQLHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "EquiDepth_DynamicDepth",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLEstimator);
            return mySQLModel;
        }
        public static SuiteData GetMySQLSD_MinDepth(JsonObject optionalTestSettings)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new MyConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MinDepthMilestoner(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                new ConstantDepth(JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize")));
            var mySQLEstimator = new JsonQueryEstimator(mySQLHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "MinDepth",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLEstimator);
            return mySQLModel;
        }
        public static SuiteData GetMySQLSD_MinDepth_DynamicDepth(JsonObject optionalTestSettings)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new MyConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MinDepthMilestoner(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                new DynamicDepth());
            var mySQLEstimator = new JsonQueryEstimator(mySQLHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "MinDepth_DynamicDepth",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLEstimator);
            return mySQLModel;
        }

        #endregion

        #region Postgres

        public static SuiteData GetPostgresSD_EquiDepth(JsonObject optionalTestSettings)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new EquiDepthMilestoner(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                new ConstantDepth(JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize")));
            var postEstimator = new JsonQueryEstimator(postHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "EquiDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postEstimator);
            return postgresModel;
        }
        public static SuiteData GetPostgresSD_EquiDepth_DynamicDepth(JsonObject optionalTestSettings)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new EquiDepthMilestoner(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                new DynamicDepth(useUniqueValueCount: false));
            var postEstimator = new JsonQueryEstimator(postHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "EquiDepth_DynamicDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postEstimator);
            return postgresModel;
        }

        public static SuiteData GetPostgresSD_MinDepth(JsonObject optionalTestSettings)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new MinDepthMilestoner(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                new ConstantDepth(JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize")));
            var postEstimator = new JsonQueryEstimator(postHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "MinDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postEstimator);
            return postgresModel;
        }
        public static SuiteData GetPostgresSD_MinDepth_DynamicDepth(JsonObject optionalTestSettings)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new MinDepthMilestoner(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                new DynamicDepth());
            var postEstimator = new JsonQueryEstimator(postHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "MinDepth_DynamicDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postEstimator);
            return postgresModel;
        }

        #endregion

        public static List<SuiteData> GetSuiteDatas(JsonObject optionalSettings)
        {
            var connectorSet = new List<SuiteData>() {
                // EquiDepth
                GetPostgresSD_EquiDepth(optionalSettings),
                GetPostgresSD_EquiDepth_DynamicDepth(optionalSettings),

                GetMySQLSD_EquiDepth(optionalSettings),
                GetMySQLSD_EquiDepth_DynamicDepth(optionalSettings),

                // MinDepth
                GetPostgresSD_MinDepth(optionalSettings),
                GetPostgresSD_MinDepth_DynamicDepth(optionalSettings),

                GetMySQLSD_MinDepth(optionalSettings),
                GetMySQLSD_MinDepth_DynamicDepth(optionalSettings)
            };

            return connectorSet;
        }


    }
}
