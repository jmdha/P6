using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Services
{
    public class SecretsService<T> where T : class
    {
        private IConfiguration configuration;

        public SecretsService()
        {
            configuration = new ConfigurationBuilder()
                .AddUserSecrets<T>()
                .Build();
        }

        public string GetConnectionString(string key)
        {
            return configuration.GetConnectionString(key);
        }

        public string GetConnectionStringValue(string connectionString, string key)
        {
            string[] split = connectionString.Split(';');
            foreach (string s in split)
            {
                if (s.Contains('='))
                    if (s.Split('=')[0] == key)
                        return s.Split('=')[1];
            }
            return "Not Found";
        }

        public bool GetLaunchOption(string key)
        {
            return bool.Parse(configuration.GetSection("LaunchSystem")[key]);
        }
    }
}
