using CampusAI.VectorPipeline.Database.Repositories;
using CampusAI.VectorPipeline.Embeddings;
using CampusAI.VectorPipeline.Services;
using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.Orchestrator
{
    //public interface IIngestionOrchestrator
    //{
    //    Task RunModuleAsync<T>(
    //        IIngestionRepository<T> repo,
    //        string moduleName,
    //        Func<T, string> toTextFunc,
    //        string indexName);
    //}
    //public class IngestionOrchestrator: IIngestionOrchestrator
    //{
    //    private readonly IEmbeddingProvider _embeddingProvider;
    //    private readonly IPineconeService _pineconeService;

    //    public IngestionOrchestrator(
    //        IEmbeddingProvider embeddingProvider,
    //        IPineconeService pineconeService)
    //    {
    //        _embeddingProvider = embeddingProvider;
    //        _pineconeService = pineconeService;
    //    }

    //    /// <summary>
    //    /// Generic ingestion for any module
    //    /// </summary>
    //    /// <typeparam name="T">Type of data row</typeparam>
    //    /// <param name="repo">Repository for fetching data</param>
    //    /// <param name="moduleName">Module name for metadata</param>
    //    /// <param name="toTextFunc">Function to convert row to text for embedding</param>
    //    /// <param name="indexName">Pinecone index name</param>
    //    public async Task RunModuleAsync<T>(
    //        IIngestionRepository<T> repo,
    //        string moduleName,
    //        Func<T, string> toTextFunc,
    //        string indexName)
    //    {
    //        // 1️⃣ Fetch data
    //        var data = await repo.FetchAsync();

    //        if (data == null || !data.Any())
    //            return;

    //        // 2️⃣ Generate embeddings
    //        var texts = data.Select(toTextFunc).ToList();
    //        var embeddings = await _embeddingProvider.GenerateEmbeddingsAsync(texts);

    //        // 3️⃣ Prepare VectorRecords
    //        var vectors = data
    //            .Select((row, i) => new VectorRecord
    //            {
    //                Id = GetRowId(row, moduleName, i), // generate unique Id
    //                Values = embeddings[i],
    //                Metadata = new Dictionary<string, object>
    //                {
    //                { "Module", moduleName },
    //                { "RowIndex", i },
    //                { "Data", row } // optional full row metadata
    //                }
    //            })
    //            .ToList();

    //        // 4️⃣ Upsert to Pinecone
    //        await _pineconeService.UpsertAsync<VectorRecord>(vectors, indexName);
    //    }

    //    /// <summary>
    //    /// Generates a unique Id for Pinecone vector
    //    /// </summary>
    //    private string GetRowId<T>(T row, string moduleName, int index)
    //    {
    //        // Try to use a property named "Id" if exists, otherwise fallback to module + index
    //        var prop = typeof(T).GetProperty("Id");
    //        if (prop != null)
    //        {
    //            var value = prop.GetValue(row);
    //            if (value != null)
    //                return $"{moduleName}-{value}";
    //        }

    //        return $"{moduleName}-{index}";
    //    }
    //}
}
