using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models
{
    public class SecretsItem
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }

        public SecretsItem(string username, string password, string server, int port)
        {
            Username = username;
            Password = password;
            Server = server;
            Port = port;
        }

        public override int GetHashCode()
        {
           return Username.GetHashCode() + Password.GetHashCode() + Server.GetHashCode() + Port;
        }
    }
}
