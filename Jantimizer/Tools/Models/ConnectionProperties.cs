using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models
{
    public class ConnectionProperties
    {
        public string ConnectionString { get; }
        public string Server { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }
        public string Database { get; }
        public string Schema { get; }

        public ConnectionProperties()
        {
            ConnectionString = "";
            Server = "";
            Port = -1;
            Username = "";
            Password = "";
            Database = "";
            Schema = "";
        }

        public ConnectionProperties(string connectionString, string server, int port, string username, string password, string database, string schema)
        {
            ConnectionString = connectionString;
            Server = server;
            Port = port;
            Username = username;
            Password = password;
            Database = database;
            Schema = schema;
        }
    }
}
