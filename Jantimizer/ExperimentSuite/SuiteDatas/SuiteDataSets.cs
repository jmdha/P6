using DatabaseConnector.Connectors;
using ExperimentSuite.Models;
using QueryEstimator;
using QueryPlanParser.Parsers;
using Milestoner.DataGatherers;
using Milestoner.DepthCalculators;
using Milestoner.Milestoners;
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

        public static Dictionary<string, Delegate> SuiteDatas = new Dictionary<string, Delegate>()
        {
            { "EquiDepth MYSQL", GetMySQLSD_EquiDepth },
            { "EquiDepth POSGRESQL", GetPostgresSD_EquiDepth },

            { "MinDepth MYSQL", GetMySQLSD_MinDepth },
            { "MinDepth POSGRESQL", GetPostgresSD_MinDepth },

            { "EquiDepth_SquareRootDynamicDepth MYSQL", GetMySQLSD_EquiDepth_DynamicDepth },
            { "EquiDepth_SquareRootDynamicDepth POSGRESQL", GetPostgresSD_EquiDepth_DynamicDepth },

            { "MinDepth_SquareRootDynamicDepth MYSQL", GetMySQLSD_MinDepth_DynamicDepth },
            { "MinDepth_SquareRootDynamicDepth POSGRESQL", GetPostgresSD_MinDepth_DynamicDepth },
        };

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
                new TotalSquareRootDynDepth(
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_YOffset"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootMultiplier"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootOffset")));
            var mySQLEstimator = new JsonQueryEstimator(mySQLHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "EquiDepth_SquareRootDynamicDepth",
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
                new UniqueSquareRootDynDepth(
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_YOffset"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootMultiplier"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootOffset")));
            var mySQLEstimator = new JsonQueryEstimator(mySQLHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "MinDepth_SquareRootDynamicDepth",
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
                new UniqueSquareRootDynDepth(
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_YOffset"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootMultiplier"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootOffset")));
            var postEstimator = new JsonQueryEstimator(postHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "EquiDepth_SquareRootDynamicDepth",
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
                new UniqueSquareRootDynDepth(
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_YOffset"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootMultiplier"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootOffset")));
            var postEstimator = new JsonQueryEstimator(postHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "MinDepth_SquareRootDynamicDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postEstimator);
            return postgresModel;
        }

        #endregion
    }
}
