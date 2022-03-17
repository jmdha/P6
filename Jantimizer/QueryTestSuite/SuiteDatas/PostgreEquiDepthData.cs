﻿using DatabaseConnector.Connectors;
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
    internal static class PostgreEquiDepthData
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
                secrets.GetConnectionStringValue(postConnectionString, "Database"),
                secrets.GetConnectionStringValue(postConnectionString, "SearchPath"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new PostgresEquiDepthHistogramManager(postConnector.ConnectionProperties, 10);
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            var postGenerator = new PostgresGenerator();
            var postgresModel = new SuiteData(
                secrets.GetLaunchOption("POSGRESQL"),
                "postgre",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager,
                postGenerator);
            return postgresModel;
        }
    }
}