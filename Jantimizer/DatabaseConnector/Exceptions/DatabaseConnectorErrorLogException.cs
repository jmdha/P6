using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;

namespace DatabaseConnector.Exceptions
{
    public class DatabaseConnectorErrorLogException : BaseErrorLogException
    {
        public string ConnectorID { get; set; }
        public string Query { get; set; }

        public DatabaseConnectorErrorLogException(Exception actualException, string connectorID, string query) : base(actualException)
        {
            ConnectorID = connectorID;
            Query = query;
        }

        public override string GetErrorLogMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Connector: {ConnectorID}");
            sb.AppendLine($"Query: {Query}");
            sb.AppendLine($"Error Message: {Message}");
            return sb.ToString();
        }
    }
}
