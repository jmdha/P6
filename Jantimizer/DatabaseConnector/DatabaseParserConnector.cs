using DatabaseConnector.Connectors;
using DatabaseConnector.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public class DatabaseParserConnector : IDatabaseParserConnector
    {
        public string Name { get; set; }
        public IDbConnector Connector { get; set; }
        public IPlanParser Parser { get; set; }

        public DatabaseParserConnector(string name, IDbConnector connector, IPlanParser parser)
        {
            Name = name;
            Connector = connector;
            Parser = parser;
        }
    }
}
