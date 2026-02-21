using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CampusAI.VectorPipeline.Embeddings
{
    public class HuggingFaceEmbeddingProvider : IEmbeddingProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;
        //private const string BaseUrl = "https://api-inference.huggingface.co/pipeline/feature-extraction/";
        //private const string BaseUrl = "https://router.huggingface.co/hf-inference/pipeline/feature-extraction/";
        private const string BaseUrl = "https://router.huggingface.co/hf-inference/models/";

        public HuggingFaceEmbeddingProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var apiKey = configuration["Embedding:HuggingFace:ApiKey"]
                ?? throw new ArgumentException(
                    "Missing HuggingFace API key in configuration (Embedding:HuggingFace:ApiKey).",
                    nameof(configuration));

            _model = configuration["Embedding:HuggingFace:Model"]
                ?? "sentence-transformers/all-MiniLM-L6-v2";

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }

        /// <summary>
        /// Generates an embedding for a single text input.
        /// Returns a float[] of 384 dimensions for all-MiniLM-L6-v2.
        /// </summary>
        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<float>();

            var payload = new { inputs = new[] { text } };
            var url = $"{BaseUrl}{_model}/pipeline/feature-extraction";

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body); // remove after confirmed working
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<List<List<float>>>(body);
            return result?[0]?.ToArray() ?? Array.Empty<float>();
        }

        /// <summary>
        /// Generates embeddings for multiple texts in a single HTTP call (efficient batch).
        /// </summary>
        public async Task<List<float[]>> GenerateBatchEmbeddingsAsync(IEnumerable<string> texts)
        {
            var textList = texts?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList()
                    ?? new List<string>();

            if (textList.Count == 0)
                return new List<float[]>();

            // ✅ Send ALL texts in ONE HTTP call
            var payload = new { inputs = textList };
            var url = $"{BaseUrl}{_model}/pipeline/feature-extraction";

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            var body = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<List<List<float>>>(body);
            return result?.Select(e => e.ToArray()).ToList() ?? new List<float[]>();
        }

        ///// <summary>
        ///// Handles both flat float[] and nested List<List<float>> responses for single input.
        ///// </summary>
        //private static float[] ParseSingleEmbedding(string json)
        //{
        //    var trimmed = json.TrimStart();

        //    // Nested array [[0.1, 0.2, ...]] — some models/endpoints wrap single result
        //    if (trimmed.StartsWith("[["))
        //    {
        //        var nested = JsonSerializer.Deserialize<List<List<float>>>(json);
        //        return nested?[0]?.ToArray() ?? Array.Empty<float>();
        //    }

        //    // Flat array [0.1, 0.2, ...]
        //    if (trimmed.StartsWith("["))
        //    {
        //        var flat = JsonSerializer.Deserialize<float[]>(json);
        //        return flat ?? Array.Empty<float>();
        //    }

        //    throw new InvalidOperationException($"Unexpected embedding response format: {json[..Math.Min(100, json.Length)]}");
        //}

        ///// <summary>
        ///// Parses batch response: always returns List<List<float>> i.e. one float[] per input.
        ///// </summary>
        //private static List<float[]> ParseBatchEmbeddings(string json)
        //{
        //    var trimmed = json.TrimStart();

        //    // Nested array [[...], [...]] — standard batch response
        //    if (trimmed.StartsWith("[["))
        //    {
        //        var nested = JsonSerializer.Deserialize<List<List<float>>>(json);
        //        return nested?.Select(e => e.ToArray()).ToList() ?? new List<float[]>();
        //    }

        //    // Edge case: single flat array returned for a single-item batch
        //    if (trimmed.StartsWith("["))
        //    {
        //        var flat = JsonSerializer.Deserialize<float[]>(json);
        //        return flat != null
        //            ? new List<float[]> { flat }
        //            : new List<float[]>();
        //    }

        //    throw new InvalidOperationException($"Unexpected batch embedding response format: {json[..Math.Min(100, json.Length)]}");
        //}
    }
}