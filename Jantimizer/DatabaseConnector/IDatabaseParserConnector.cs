using DatabaseConnector.Connectors;
using DatabaseConnector.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public interface IDatabaseParserConnector
    {
        public string Name { get; set; }
        public IDbConnector Connector { get; set; }
        public IPlanParser Parser { get; set; }
    }
}
