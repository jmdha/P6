using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;

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
            if (!configuration.GetSection("ConnectionStrings").GetSection(key).Exists())
                throw new MissingKeyException($"Error, key missing '{key}' for launch option!");
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
            if (!configuration.GetSection("LaunchSystem").GetSection(key).Exists())
                throw new MissingKeyException($"Error, key missing '{key}' for launch option!");
            return bool.Parse(configuration.GetSection("LaunchSystem")[key]);
        }

        public bool GetAutoStartOption(string key)
        {
            if (!configuration.GetSection("AutoStart").GetSection(key).Exists())
                throw new MissingKeyException($"Error, key missing '{key}' for auto start option!");
            return bool.Parse(configuration.GetSection("AutoStart")[key]);
        }
    }
}
