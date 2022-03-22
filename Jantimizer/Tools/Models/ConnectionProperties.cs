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

        public ConnectionProperties(SecretsItem secrets, string database, string schema)
        {
            Secrets = secrets;
            Database = database;
            Schema = schema;
        }

        public void Update(ConnectionProperties settings)
        {
            Database = settings.Database;
            Schema = settings.Schema;
        }
    }
}
