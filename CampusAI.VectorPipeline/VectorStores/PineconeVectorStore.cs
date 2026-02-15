using CampusAI.VectorPipeline.Vector;
using Microsoft.Extensions.Configuration;
using Pinecone;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CampusAI.VectorPipeline.VectorStores
{
    public class PineconeVectorStore : IVectorStore
    {
        private readonly PineconeClient _client;
        private readonly string _indexName;

        public PineconeVectorStore(IConfiguration config)
        {
            _client = new PineconeClient(config["Pinecone:ApiKey"]);
            _indexName = config["Pinecone:IndexName"];
        }

        public async Task UpsertBatchAsync(List<VectorRecord> records)
        {
            var index = _client.Index(_indexName);

            var vectors = records.Select(r => new Pinecone.Vector
            {
                Id = r.Id,
                Values = r.Values,
                Metadata = new Metadata
                {
                    ["genre"] = "action",   // string
                    ["year"] = 2024        // int/double also supported
                }
            }).ToArray();


            await index.UpsertAsync(new UpsertRequest { Vectors = vectors });
        }
    }
}
