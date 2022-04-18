using DatabaseConnector.Connectors;
using ExperimentSuite.Models;
using Histograms.DataGatherers;
using Histograms.Managers;
using QueryOptimiser;
using QueryParser;
using QueryParser.QueryParsers;
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
        public static SuiteData GetMySQLSD_Default(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new EquiDepthHistogramManager(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties), 
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { });
            if (additionalParser != null)
                mySQLParserManager.QueryParsers.Add(additionalParser);
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "Default",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }

        public static SuiteData GetMySQLSD_EquiDepth(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new EquiDepthHistogramManager(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { });
            if (additionalParser != null)
                mySQLParserManager.QueryParsers.Add(additionalParser);
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "EquiDepth",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }

        public static SuiteData GetMySQLSD_EquiDepthVariance(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new EquiDepthVarianceHistogramManager(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var mySQLOptimiser = new QueryOptimiserEquiDepthVariance(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { });
            if (additionalParser != null)
                mySQLParserManager.QueryParsers.Add(additionalParser);
            var mySQLModel = new SuiteData(
                new TestSettings(mySQLConnectionProperties),
                "EquiDepthVariance",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }
        public static SuiteData GetMySQLSD_MinDepth(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MinDepthHistogramManager(
                new MySqlDataGatherer(mySQLConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { });
            if (additionalParser != null)
                mySQLParserManager.QueryParsers.Add(additionalParser);
            var mySQLModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, true, true, mySQLConnectionProperties),
                "MinDepth",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }

        #endregion

        #region Postgres

        public static SuiteData GetPostgresSD_Default(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new EquiDepthHistogramManager(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            if (additionalParser != null)
                postParserManager.QueryParsers.Add(additionalParser);
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "Default",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }

        public static SuiteData GetPostgresSD_EquiDepth(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new EquiDepthHistogramManager(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            if (additionalParser != null)
                postParserManager.QueryParsers.Add(additionalParser);
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "EquiDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }

        public static SuiteData GetPostgresSD_EquiDepthVariance(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new EquiDepthVarianceHistogramManager(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var postOptimiser = new QueryOptimiserEquiDepthVariance(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            if (additionalParser != null)
                postParserManager.QueryParsers.Add(additionalParser);
            var postgresModel = new SuiteData(
                new TestSettings(postConnectionProperties),
                "EquiDepthVariance",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }
        public static SuiteData GetPostgresSD_MinDepth(JsonObject optionalTestSettings, IQueryParser? additionalParser = null)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new MinDepthHistogramManager(
                new PostgresDataGatherer(postConnector.ConnectionProperties),
                JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"));
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            if (additionalParser != null)
                postParserManager.QueryParsers.Add(additionalParser);
            var postgresModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, true, true, postConnectionProperties),
                "MinDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }

        #endregion

        public static List<SuiteData> GetSuiteDatas(JsonObject optionalSettings)
        {
            var pgDataDefault = GetPostgresSD_Default(optionalSettings);
            var myDataDefault = GetMySQLSD_Default(optionalSettings, pgDataDefault.QueryParserManager.QueryParsers[0]);

            var pgDataEquiDepth = GetPostgresSD_EquiDepth(optionalSettings);
            var myDataEquiDepth = GetMySQLSD_EquiDepth(optionalSettings, pgDataEquiDepth.QueryParserManager.QueryParsers[0]);

            var pgDataEquiDepthVariance = GetPostgresSD_EquiDepthVariance(optionalSettings);
            var myDataEquiDepthVariance = GetMySQLSD_EquiDepthVariance(optionalSettings, pgDataEquiDepthVariance.QueryParserManager.QueryParsers[0]);

            var pgDataMinDepth = GetPostgresSD_MinDepth(optionalSettings);
            var myDataMinDepth = GetMySQLSD_MinDepth(optionalSettings, pgDataMinDepth.QueryParserManager.QueryParsers[0]);

            var connectorSet = new List<SuiteData>() {
                pgDataDefault,
                myDataDefault,
                pgDataEquiDepth,
                myDataEquiDepth,
                pgDataEquiDepthVariance,
                myDataEquiDepthVariance,
                pgDataMinDepth,
                myDataMinDepth
            };
            return connectorSet;
        }


    }
}
