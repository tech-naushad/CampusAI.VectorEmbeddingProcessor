using CampusAI.VectorPipeline.Vector;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.VectorStores
{
    public class QdrantVectorStore : IVectorStore
    {
        private readonly string _endpoint;

        public QdrantVectorStore(IConfiguration config)
        {
            _endpoint = config["Qdrant:Endpoint"];
        }

        public async Task UpsertBatchAsync(List<VectorRecord> records)
        {
            // Call Qdrant REST API here
            // Map VectorRecord to Qdrant format
        }
    }
}
