using DatabaseConnector.Connectors;
using Histograms.Managers;
using QueryOptimiser;
using QueryParser;
using QueryParser.QueryParsers;
using QueryPlanParser.Parsers;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Services;

namespace QueryTestSuite.SuiteDatas
{
    internal static class SuiteDataSets
    {
        internal static SecretsService<Program> secrets = new SecretsService<Program>();
        public static SuiteData GetMySQLSD(IQueryParser? additionalParser = null)
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
                secrets.GetLaunchOption("MYSQL"),
                "mysql",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }

        public static SuiteData GetPostgresSD(IQueryParser? additionalParser = null)
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
                secrets.GetLaunchOption("POSGRESQL"),
                "postgre",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager);
            return postgresModel;
        }
    }
}
