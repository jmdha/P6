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

        public SecretsItem(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
