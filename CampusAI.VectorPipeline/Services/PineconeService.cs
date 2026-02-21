using CampusAI.VectorPipeline.Configuration;
using CampusAI.VectorPipeline.Embeddings;
using CampusAI.VectorPipeline.Vector;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CampusAI.VectorPipeline.Services
{
    public interface IPineconeService
    {
        //Task UpsertAsync<T>(IEnumerable<VectorRecord> records, string indexName);
        Task UpsertAsync(IEnumerable<VectorRecord> records, string? ns = null);
    }
    public class PineconeService : IPineconeService
    {
        private readonly HttpClient _httpClient;
        private readonly PineconeOptions _options;

        public PineconeService(HttpClient httpClient, IOptions<PineconeOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            //_httpClient.BaseAddress = new Uri($"https://{_options.Host}");
            _httpClient.DefaultRequestHeaders.Add("Api-Key", _options.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-Pinecone-API-Version", "2024-07");
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        /// <summary>
        /// Upserts VectorRecords into Pinecone in batches of 100.
        /// Namespace is created automatically if it doesn't exist.
        /// </summary>
        public async Task UpsertAsync(IEnumerable<VectorRecord> records, string? ns = null)
        {
            if (records == null) return;

            var batches = records
                .Select((r, i) => new { r, i })
                .GroupBy(x => x.i / 100)
                .Select(g => g.Select(x => x.r).ToList());

            foreach (var batch in batches)
            {
                await UpsertBatchAsync(batch, ns);
            }
        }

        private async Task UpsertBatchAsync(List<VectorRecord> batch, string? ns)
        {
            var payload = new PineconeUpsertRequest
            {
                Vectors = batch.Select(r => new PineconeVector
                {
                    Id = r.Id,
                    Values = r.Values,
                    Metadata = r.Metadata
                }).ToList(),
                Namespace = ns
            };

            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // To this:
            var url = $"{_options.Host}/vectors/upsert";
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Pinecone upsert failed {(int)response.StatusCode}: {error}");
            }
        }

        // ─── Request models ────────────────────────────────────────────────────

        private class PineconeUpsertRequest
        {
            [JsonPropertyName("vectors")]
            public List<PineconeVector> Vectors { get; set; } = new();

            [JsonPropertyName("namespace")]
            public string? Namespace { get; set; }
        }

        private class PineconeVector
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("values")]
            public float[] Values { get; set; } = Array.Empty<float>();

            [JsonPropertyName("metadata")]
            public Dictionary<string, object>? Metadata { get; set; }
        }
    }
}
