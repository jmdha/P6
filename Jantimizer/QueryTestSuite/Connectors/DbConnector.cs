using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresTestSuite.Connectors
{
    internal abstract class DbConnector
    {
        public string Name { get; set; }

        public DbConnector(string name)
        {
            Name = name;
        }

        //public abstract string GetQueryPlan(string query);
        //public abstract string CallQuery(string query);
        //public abstract string GetCardinalityEstimate(string query);

    }
}
