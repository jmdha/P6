using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Services
{
    public class SecretsService
    {
        private IConfiguration configuration;

        public SecretsService()
        {
            configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
        }

        public string GetConnectionString(string key)
        {
            return configuration.GetConnectionString(key);
        }
    }
}
