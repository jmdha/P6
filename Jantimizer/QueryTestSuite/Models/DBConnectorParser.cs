using DatabaseConnector;
using QueryPlanParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    internal class DBConnectorParser
    {
        public string Name { get; set; }
        public IDbConnector Connector { get; set; }
        public IPlanParser Parser { get; set; }

        public DBConnectorParser(string name, IDbConnector connector, IPlanParser parser)
        {
            Name = name;
            Connector = connector;
            Parser = parser;
        }
    }
}
