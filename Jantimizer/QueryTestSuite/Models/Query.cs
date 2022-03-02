using QueryTestSuite.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    /// <summary>
    /// Purpose of class: To have the same query be executable on multiple SQL servers, despite slight syntax variations.
    /// </summary>
    public class Query
    {
        public int Name { get; set; }

        public Dictionary<Type, string> Variantions { get; set; } = new Dictionary<Type, string>();

        public Query() { }
        public Query(string query)
        {
            AddVariation<DbConnector>(query);
        }
        public Query(Dictionary<Type, string> variations)
        {
            Variantions = variations;
        }

        public void AddVariation<connector>(string query) where connector : DbConnector
        {
            Variantions[typeof(connector)] = query;
        }


        public string GetVariation() => GetVariation(typeof(DbConnector));
        public string GetVariation(DbConnector connector) => GetVariation(connector.GetType());
        public string GetVariation(Type connectorType) => Variantions[connectorType];
    }
}
