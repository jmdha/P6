using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class JsonQuery
    {
        public List<JoinNode> JoinNodes { get; set; }
        public string EquivalentSQLQuery { get; set; }
        public List<INode> Nodes { get {
                var newList = new List<INode>();
                newList.AddRange(JoinNodes);
                return newList;
            } 
        }

        public JsonQuery()
        {
            JoinNodes = new List<JoinNode>();
            EquivalentSQLQuery = "";
        }

        public JsonQuery(List<JoinNode> joinNodes, string equivalentSQLQuery)
        {
            JoinNodes = joinNodes;
            EquivalentSQLQuery = equivalentSQLQuery;
        }

        public JsonQuery(string fileText)
        {
            var jsonQuery = JsonSerializer.Deserialize<JsonQuery>(fileText);
            if (jsonQuery != null)
            {
                JoinNodes = jsonQuery.JoinNodes;
                EquivalentSQLQuery = jsonQuery.EquivalentSQLQuery;
            }
            else
                throw new JsonException("Could not parse the json file!");
        }
    }
}
