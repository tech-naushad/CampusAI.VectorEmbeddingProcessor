using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CampusAI.VectorPipeline.Configuration
{
    public class ModuleConfiguration
    {
        [JsonPropertyName("moduleName")]
        public string ModuleName { get; set; }

        [JsonPropertyName("batchSize")]
        public int BatchSize { get; set; } = 50;

        [JsonPropertyName("maxRetries")]
        public int MaxRetries { get; set; } = 3;

        [JsonPropertyName("retryDelayMs")]
        public int RetryDelayMs { get; set; } = 500;
    }
}
