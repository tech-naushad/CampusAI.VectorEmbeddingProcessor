using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CampusAI.VectorPipeline.Database.Factory
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(DatabaseType dbType);
    }
}
