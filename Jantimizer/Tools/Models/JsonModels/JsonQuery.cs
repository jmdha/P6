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
        public List<FilterNode> FilterNodes { get; set; }
        public string EquivalentSQLQuery { get; set; }
        public List<INode> Nodes { get {
                var newList = new List<INode>();
                newList.AddRange(JoinNodes);
                newList.AddRange(FilterNodes);
                return newList;
            } 
        }

        public JsonQuery(List<JoinNode> joinNodes, List<FilterNode> filterNodes, string equivalentSQLQuery)
        {
            JoinNodes = joinNodes;
            FilterNodes = filterNodes;
            EquivalentSQLQuery = equivalentSQLQuery;
        }

        public JsonQuery(string fileText)
        {
            var jsonQuery = JsonSerializer.Deserialize<JsonQuery>(fileText);
            if (jsonQuery != null)
            {
                JoinNodes = jsonQuery.JoinNodes;
                FilterNodes = jsonQuery.FilterNodes;
                EquivalentSQLQuery = jsonQuery.EquivalentSQLQuery;
            }
            else
                throw new JsonException("Could not parse the json file!");
        }
    }
}
