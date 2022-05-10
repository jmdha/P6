using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

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

        public bool HasKey(string key)
        {
            if (configuration.GetSection("ConnectionProperties").Exists())
                return configuration.GetSection("ConnectionProperties").GetSection(key).Exists();
            return false;
        }

        public SecretsItem GetSecretsItem(string key)
        {
            return new SecretsItem(
                configuration.GetSection("ConnectionProperties").GetSection(key)["Username"],
                configuration.GetSection("ConnectionProperties").GetSection(key)["Password"],
                configuration.GetSection("ConnectionProperties").GetSection(key)["Server"],
                int.Parse(configuration.GetSection("ConnectionProperties").GetSection(key)["Port"]));
        }
    }
}
