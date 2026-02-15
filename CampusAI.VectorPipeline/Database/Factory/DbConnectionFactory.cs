using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CampusAI.VectorPipeline.Database.Factory
{
    public class DbConnectionFactory: IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection(DatabaseType dbType)
        {
            string connectionString = dbType switch
            {
                DatabaseType.CampusDb => _configuration.GetConnectionString("CampusDb"),
                DatabaseType.LoggingDb => _configuration.GetConnectionString("LoggingDb"),
                DatabaseType.AnalyticsDb => _configuration.GetConnectionString("AnalyticsDb"),
                _ => throw new ArgumentOutOfRangeException(nameof(dbType), "Unknown database type")
            };

            return new SqlConnection(connectionString);
        }
    }
}
