using Histograms.Managers;
using QueryGenerator.QueryGenerators;
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
            string mySQLConnectionString = secrets.GetConnectionString("MYSQL");
            var mySQLConnectionProperties = new ConnectionProperties(
                mySQLConnectionString,
                secrets.GetConnectionStringValue(mySQLConnectionString, "Server"),
                int.Parse(secrets.GetConnectionStringValue(mySQLConnectionString, "Port")),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Uid"),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Pwd"),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Database"),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Database"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MySQLEquiDepthHistogramManager(mySQLConnector.ConnectionProperties, 10);
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
            var mySQLGenerator = new MySQLGenerator();
            var mySQLModel = new SuiteData(
                secrets.GetLaunchOption("MYSQL"),
                "mysql",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager,
                mySQLGenerator);
            return mySQLModel;
        }
    }
}
