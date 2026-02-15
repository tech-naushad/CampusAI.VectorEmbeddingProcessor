using CampusAI.VectorPipeline.Configuration;
using CampusAI.VectorPipeline.Embeddings;
using CampusAI.VectorPipeline.Vector;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace CampusAI.VectorPipeline.Services
{
    public interface IPineconeService
    {
        //Task UpsertAsync<T>(IEnumerable<VectorRecord> records, string indexName);
        Task UpsertAsync(IEnumerable<VectorRecord> records, string indexName, string? ns = null);
    }
    public class PineconeService : IPineconeService
    {
        private readonly HttpClient _httpClient;
        private readonly PineconeOptions _options;

        public PineconeService(
            HttpClient httpClient,
            IOptions<PineconeOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _httpClient.BaseAddress =
                new Uri($"https://{_options.IndexName}-{_options.Environment}.svc.pinecone.io");

            _httpClient.DefaultRequestHeaders.Add("Api-Key", _options.ApiKey);
        }

        /// <summary>
        /// Upserts a batch of VectorRecords into Pinecone
        /// </summary>
        /// <param name="records">Vectors to insert</param>
        /// <param name="indexName">Pinecone index name</param>
        /// <param name="ns">Optional namespace; if null uses default from options</param>
        public async Task UpsertAsync(
            IEnumerable<VectorRecord> records,
            string indexName,
            string? ns = null)
        {
            if (records == null) return;

            var namespaceToUse = ns ?? _options.Namespace;

            var requestUrl = $"https://{indexName}-{_options.Environment}.svc.pinecone.io/vectors/upsert";

            // Pinecone expects JSON like { "vectors": [...], "namespace": "..." }
            var payload = new
            {
                vectors = records,
                @namespace = namespaceToUse
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(requestUrl, content);

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Helper: upsert any IVectorizable entities directly
        /// </summary>
        public async Task UpsertAsync<T>(
            IEnumerable<T> entities,
            IEmbeddingProvider embeddingProvider,
            string indexName,
            string? ns = null) where T : IVectorizable
        {
            var vectorRecords = await Vectorizer.ToVectorRecordsAsync(entities, embeddingProvider);
            await UpsertAsync(vectorRecords, indexName, ns);
        }
    }
}
