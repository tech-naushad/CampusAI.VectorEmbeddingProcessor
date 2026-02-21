using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace CampusAI.VectorPipeline.Embeddings
{
    public class OpenAIEmbeddingProvider : IEmbeddingProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public OpenAIEmbeddingProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var apiKey = configuration["Embedding:OpenAI:ApiKey"];
            _model = configuration["Embedding:OpenAI:Model"] ?? "text-embedding-3-small";

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var payload = new
            {
                model = _model,
                input = text
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/embeddings", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAIEmbeddingResponse>();
            return result!.Data[0].Embedding;
        }

        public async Task<List<float[]>> GenerateBatchEmbeddingsAsync(IEnumerable<string> texts)
        {
            var list = new List<float[]>();

            foreach (var text in texts)
            {
                list.Add(await GenerateEmbeddingAsync(text));
            }

            return list;
        }
    }

    // Example response model
    public class OpenAIEmbeddingResponse
    {
        public List<OpenAIEmbeddingData> Data { get; set; } = new();
    }

    public class OpenAIEmbeddingData
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
