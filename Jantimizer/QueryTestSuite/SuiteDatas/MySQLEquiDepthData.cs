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
    internal static class MySQLEquiDepthData
    {
        public static SuiteData GetData<T>(SecretsService<T> secrets) where T : class
        {
            string postConnectionString = secrets.GetConnectionString("POSGRESQL");
            var postConnectionProperties = new ConnectionProperties(
                postConnectionString,
                secrets.GetConnectionStringValue(postConnectionString, "Host"),
                int.Parse(secrets.GetConnectionStringValue(postConnectionString, "Port")),
                secrets.GetConnectionStringValue(postConnectionString, "Username"),
                secrets.GetConnectionStringValue(postConnectionString, "Password"),
                "",
                "");
            var postConnector = new PostgreSqlConnector(postConnectionProperties);

            string mySQLConnectionString = secrets.GetConnectionString("MYSQL");
            var mySQLConnectionProperties = new ConnectionProperties(
                mySQLConnectionString,
                secrets.GetConnectionStringValue(mySQLConnectionString, "Server"),
                int.Parse(secrets.GetConnectionStringValue(mySQLConnectionString, "Port")),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Uid"),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Pwd"),
                "",
                "");
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MySQLEquiDepthHistogramManager(mySQLConnector.ConnectionProperties, 10);
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            var mySQLModel = new SuiteData(
                new TestSettings(),
                secrets.GetLaunchOption("MYSQL"),
                "mysql",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager);
            return mySQLModel;
        }
    }
}
