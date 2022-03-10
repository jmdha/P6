using DatabaseConnector.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public class DatabaseConnector : IDatabaseConnector
    {
        public string Name { get; set; }
        public IDbConnector Connector { get; set; }

        public DatabaseConnector(string name, IDbConnector connector)
        {
            Name = name;
            Connector = connector;
        }
    }
}
