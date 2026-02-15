using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.VectorStores
{
    public interface IVectorStore
    {
        Task UpsertBatchAsync(List<VectorRecord> records);
    }

}
