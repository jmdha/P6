using DatabaseConnector.Connectors;
using ExperimentSuite.Models;
using Histograms.Managers;
using QueryOptimiser;
using QueryParser;
using QueryParser.QueryParsers;
using QueryPlanParser.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Services;

namespace ExperimentSuite.SuiteDatas
{
    internal static class SuiteDataSets
    {
        internal static SecretsService<MainWindow> secrets = new SecretsService<MainWindow>();
        public static SuiteData GetMySQLSD_Default(IQueryParser? additionalParser = null)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MySQLEquiDepthHistogramManager(mySQLConnector.ConnectionProperties, 10);
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { });
            if (additionalParser != null)
                mySQLParserManager.QueryParsers.Add(additionalParser);
            var mySQLModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, mySQLConnectionProperties),
                "Default",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }

        public static SuiteData GetMySQLSD_EquiDepth(IQueryParser? additionalParser = null)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MySQLEquiDepthHistogramManager(mySQLConnector.ConnectionProperties, 10);
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { });
            if (additionalParser != null)
                mySQLParserManager.QueryParsers.Add(additionalParser);
            var mySQLModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, mySQLConnectionProperties),
                "EquiDepth",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }

        public static SuiteData GetMySQLSD_EquiDepthVariance(IQueryParser? additionalParser = null)
        {
            var mySQLConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MySQLEquiDepthVarianceHistogramManager(mySQLConnector.ConnectionProperties, 10);
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { });
            if (additionalParser != null)
                mySQLParserManager.QueryParsers.Add(additionalParser);
            var mySQLModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, mySQLConnectionProperties),
                "EquiDepthVariance",
                "MYSQL",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }

        public static SuiteData GetPostgresSD_Default(IQueryParser? additionalParser = null)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new PostgresEquiDepthHistogramManager(postConnector.ConnectionProperties, 10);
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            if (additionalParser != null)
                postParserManager.QueryParsers.Add(additionalParser);
            var postgresModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, postConnectionProperties),
                "Default",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }

        public static SuiteData GetPostgresSD_EquiDepth(IQueryParser? additionalParser = null)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new PostgresEquiDepthHistogramManager(postConnector.ConnectionProperties, 10);
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            if (additionalParser != null)
                postParserManager.QueryParsers.Add(additionalParser);
            var postgresModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, postConnectionProperties),
                "EquiDepth",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }

        public static SuiteData GetPostgresSD_EquiDepthVariance(IQueryParser? additionalParser = null)
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new PostgresEquiDepthVarianceHistogramManager(postConnector.ConnectionProperties, 10);
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            if (additionalParser != null)
                postParserManager.QueryParsers.Add(additionalParser);
            var postgresModel = new SuiteData(
                new TestSettings(true, true, true, true, true, true, postConnectionProperties),
                "EquiDepthVariance",
                "POSGRESQL",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }
    }
}
