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

        public SecretsItem GetSecretsItem(string key)
        {
            return new SecretsItem(
                configuration.GetSection("ConnectionProperties").GetSection(key)["Username"],
                configuration.GetSection("ConnectionProperties").GetSection(key)["Password"]);
        }

        public bool GetLaunchOption(string key)
        {
            return bool.Parse(configuration.GetSection("LaunchSystem")[key]);
        }
    }
}
