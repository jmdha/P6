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
using DatabaseConnector;
using QueryPlanParser;
using Milestoner;
using Tools.Models.JsonModels;

namespace ExperimentSuite.SuiteDatas
{
    internal static class SuiteDataSets
    {
        internal static SecretsService<MainWindow> secrets = new SecretsService<MainWindow>();

        private static Dictionary<string, Delegate> _connectionProps = new Dictionary<string, Delegate>()
        {
            { "MYSQL", GetMySQLConnectionProperties },
            { "POSGRESQL", GetPostgresConnectionProperties }
        };

        private static Dictionary<string, Delegate> _connectors = new Dictionary<string, Delegate>()
        {
            { "MYSQL", GetMySQLConnector },
            { "POSGRESQL", GetPostgresConnector },
        };

        private static Dictionary<string, Delegate> _parsers = new Dictionary<string, Delegate>()
        {
            { "MYSQL", GetMySQLPlanParser },
            { "POSGRESQL", GetPostgresPlanParser }
        };

        private static Dictionary<string, Delegate> _milestoners = new Dictionary<string, Delegate>()
        {
            { "EquiDepth", GetMilestoner_EquiDepth },
            { "MinDepth", GetMilestoner_MinDepth },
            { "EquiDepth_SquareDynDepth", GetMilestoner_EquiDepth_Square },
            { "MinDepth_SquareDynDepth", GetMilestoner_MinDepth_Square },
            { "MinDepth_LogarithmicDepth", GetMilestoner_MinDepth_LogarithmicDepth },
            { "MinDepth_LogarithmicCount", GetMilestoner_MinDepth_LogarithmicCount },
            { "MinDepth_LinearDepth", GetMilestoner_MinDepth_LinearDepth }
        };

        private static Dictionary<string, Delegate> _dataGatheres = new Dictionary<string, Delegate>()
        {
            { "MYSQL", GetMySQLDataGathere },
            { "POSGRESQL", GetPostgresDataGathere }
        };

        private static Dictionary<string, Delegate> _estimators = new Dictionary<string, Delegate>()
        {
            { "JSON", GetJsonEstimator }
        };

        public static SuiteData BuildSuiteData(string connectorName, string milestoneName, JsonObject optionalTestSettings)
        {
            var connProperties = _connectionProps[connectorName].DynamicInvoke() as ConnectionProperties;
            if (connProperties == null)
                throw new ArgumentNullException("Could not build suite data!");
            var connector = _connectors[connectorName].DynamicInvoke(connProperties) as IDbConnector;
            if (connector == null)
                throw new ArgumentNullException("Could not build suite data!");
            var parser = _parsers[connectorName].DynamicInvoke() as IPlanParser;
            if (parser == null)
                throw new ArgumentNullException("Could not build suite data!");

            var dataGathere = _dataGatheres[connectorName].DynamicInvoke(connProperties) as IDataGatherer;
            if (dataGathere == null)
                throw new ArgumentNullException("Could not build suite data!");
            var milestoner = _milestoners[milestoneName].DynamicInvoke(dataGathere, optionalTestSettings) as IMilestoner;
            if (milestoner == null)
                throw new ArgumentNullException("Could not build suite data!");

            var estimator = _estimators["JSON"].DynamicInvoke(milestoner, optionalTestSettings) as IQueryEstimator<JsonQuery>;
            if (estimator == null)
                throw new ArgumentNullException("Could not build suite data!");

            return new SuiteData(
                new TestSettings(connProperties),
                milestoneName,
                connectorName,
                connector,
                parser,
                milestoner,
                estimator
                );
        }

        // Connection properties
        private static ConnectionProperties GetMySQLConnectionProperties() => new ConnectionProperties(secrets.GetSecretsItem("MYSQL"));
        private static ConnectionProperties GetPostgresConnectionProperties() => new ConnectionProperties(secrets.GetSecretsItem("POSGRESQL"));
        
        // Connectors
        private static IDbConnector GetMySQLConnector(ConnectionProperties props) => new MyConnector(props);
        private static IDbConnector GetPostgresConnector(ConnectionProperties props) => new PostgreSqlConnector(props);
        
        // Plan Parsers
        private static IPlanParser GetMySQLPlanParser() => new MySQLParser();
        private static IPlanParser GetPostgresPlanParser() => new PostgreSqlParser();
        
        // Data Gatheres
        private static IDataGatherer GetMySQLDataGathere(ConnectionProperties props) => new MySqlDataGatherer(props);
        private static IDataGatherer GetPostgresDataGathere(ConnectionProperties props) => new PostgresDataGatherer(props);

        // Estimators
        private static IQueryEstimator<JsonQuery> GetJsonEstimator(IMilestoner milestoner, JsonObject optionalTestSettings) => new JsonQueryEstimator(milestoner, JsonHelper.GetValue<int>(optionalTestSettings, "MaxEstimatorSweeps"));

        // Milestoners
        private static IMilestoner GetMilestoner_EquiDepth(IDataGatherer gathere, JsonObject optionalTestSettings)
        {
            return new EquiDepthMilestoner(
                gathere,
                new ConstantDepth(JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"))
            );
        }

        private static IMilestoner GetMilestoner_MinDepth(IDataGatherer gathere, JsonObject optionalTestSettings)
        {
            return new MinDepthMilestoner(
                gathere,
                new ConstantDepth(JsonHelper.GetValue<int>(optionalTestSettings, "BucketSize"))
            );
        }

        private static IMilestoner GetMilestoner_EquiDepth_Square(IDataGatherer gathere, JsonObject optionalTestSettings)
        {
            return new EquiDepthMilestoner(
                gathere,
                new SquareRootDynDepth(
                    JsonHelper.GetValue<bool>(optionalTestSettings,   "DynDepth_BasedOnUnique"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_YOffset"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootMultiplier"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootOffset")
                )
            );
        }

        private static IMilestoner GetMilestoner_MinDepth_Square(IDataGatherer gathere, JsonObject optionalTestSettings)
        {
            return new MinDepthMilestoner(
                gathere,
                new SquareRootDynDepth(
                    JsonHelper.GetValue<bool>(optionalTestSettings,   "DynDepth_BasedOnUnique"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_YOffset"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootMultiplier"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "SquareRootDynDepth_RootOffset")
                )
             );
        }

        private static IMilestoner GetMilestoner_MinDepth_LogarithmicDepth(IDataGatherer gathere, JsonObject optionalTestSettings)
        {
            return new MinDepthMilestoner(
                gathere,
                new LogarithmicDepth(
                    JsonHelper.GetValue<bool>(optionalTestSettings, "DynDepth_BasedOnUnique"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "LogDynDepth_LogBase"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "LogDynDepth_Multiplier")
                )
            );
        }

        private static IMilestoner GetMilestoner_MinDepth_LogarithmicCount(IDataGatherer gathere, JsonObject optionalTestSettings)
        {
            return new MinDepthMilestoner(
                gathere,
                new LogarithmicCount(
                    JsonHelper.GetValue<bool>(optionalTestSettings, "DynDepth_BasedOnUnique"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "LogDynDepth_LogBase"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "LogDynDepth_Multiplier")
                )
            );
        }

        private static IMilestoner GetMilestoner_MinDepth_LinearDepth(IDataGatherer gathere, JsonObject optionalTestSettings)
        {
            return new MinDepthMilestoner(
                gathere,
                new LinearDepth(
                    JsonHelper.GetValue<bool>(optionalTestSettings,   "DynDepth_BasedOnUnique"),
                    JsonHelper.GetValue<double>(optionalTestSettings, "LinearDynDepth_Multiplier"),
                    JsonHelper.GetValue<int>(optionalTestSettings,    "LinearDynDepth_YOffset")
                )
            );
        }
    }
}
