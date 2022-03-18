using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tools.Models
{
    public class ConnectionProperties
    {
        public string Server { get; set; }
        public int Port { get; set; }
        [JsonIgnore]
        public SecretsItem Secrets { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }

        public ConnectionProperties()
        {
            Server = "";
            Port = -1;
            Secrets = new SecretsItem();
            Database = "";
            Schema = "";
        }

        public ConnectionProperties(SecretsItem secrets)
        {
            Server = "";
            Port = -1;
            Secrets = secrets;
            Database = "";
            Schema = "";
        }

        public ConnectionProperties(string server, int port, SecretsItem secrets, string database, string schema)
        {
            Server = server;
            Port = port;
            Secrets = secrets;
            Database = database;
            Schema = schema;
        }
    }
}
