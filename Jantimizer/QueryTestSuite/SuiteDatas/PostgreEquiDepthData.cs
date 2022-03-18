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
    internal static class PostgreEquiDepthData
    {
        public static SuiteData GetData<T>(SecretsService<T> secrets) where T : class
        {
            var postConnectionProperties = new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new PostgresEquiDepthHistogramManager(postConnector.ConnectionProperties, 10);
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            var postgresModel = new SuiteData(
                new TestSettings(true, true, true, postConnectionProperties),
                secrets.GetLaunchOption("POSGRESQL"),
                secrets.GetAutoStartOption("POSGRESQL"),
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
