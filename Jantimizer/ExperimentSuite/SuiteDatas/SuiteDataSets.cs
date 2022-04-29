using DatabaseConnector.Connectors;
using ExperimentSuite.Models;
using Histograms.DataGatherers;
using Histograms.Managers;
using QueryEstimator;
using QueryPlanParser.Parsers;
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
        public static SuiteData GetMySQLSD_Default(JsonObject optionalTestSettings)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new MyConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new EquiDepthHistogramManager(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties), 
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var mySQLEstimator = new JsonQueryEstimator(mySQLHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "Default",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLEstimator);
            return mySQLModel;
        }

        public static SuiteData GetMySQLSD_EquiDepth(JsonObject optionalTestSettings)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new MyConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new EquiDepthHistogramManager(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
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

        public static SuiteData GetMySQLSD_MinDepth(JsonObject optionalTestSettings)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new MyConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MinDepthHistogramManager(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
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

        #endregion

        #region Postgres

        public static SuiteData GetPostgresSD_Default(JsonObject optionalTestSettings)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new EquiDepthHistogramManager(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var postEstimator = new JsonQueryEstimator(postHistoManager, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "Default",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postEstimator);
            return postgresModel;
        }

        public static SuiteData GetPostgresSD_EquiDepth(JsonObject optionalTestSettings)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new EquiDepthHistogramManager(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
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

        public static SuiteData GetPostgresSD_MinDepth(JsonObject optionalTestSettings)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new MinDepthHistogramManager(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
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

        #endregion

        public static List<SuiteData> GetSuiteDatas(JsonObject optionalSettings)
        {
            var pgDataDefault = GetPostgresSD_Default(optionalSettings);
            var myDataDefault = GetMySQLSD_Default(optionalSettings);

            var pgDataEquiDepth = GetPostgresSD_EquiDepth(optionalSettings);
            var myDataEquiDepth = GetMySQLSD_EquiDepth(optionalSettings);

            var pgDataMinDepth = GetPostgresSD_MinDepth(optionalSettings);
            var myDataMinDepth = GetMySQLSD_MinDepth(optionalSettings);

            var connectorSet = new List<SuiteData>() {
                pgDataDefault,
                myDataDefault,
                pgDataEquiDepth,
                myDataEquiDepth,
                pgDataMinDepth,
                myDataMinDepth
            };
            return connectorSet;
        }


    }
}
