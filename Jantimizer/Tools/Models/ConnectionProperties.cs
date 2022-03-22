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
        public string? Server { get; set; }
        public int? Port { get; set; }
        [JsonIgnore]
        public SecretsItem? Secrets { get; set; }
        public string? Database { get; set; }
        public string? Schema { get; set; }

        public ConnectionProperties()
        {
        }

        public ConnectionProperties(SecretsItem secrets)
        {
            Secrets = secrets;
        }

        public ConnectionProperties(string server, int port, SecretsItem secrets, string database, string schema)
        {
            Server = server;
            Port = port;
            Secrets = secrets;
            Database = database;
            Schema = schema;
        }

        public void Update(ConnectionProperties settings)
        {
            Server = settings.Server;
            Port = settings.Port;
            Database = settings.Database;
            Schema = settings.Schema;
        }
    }
}
