using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.Embeddings
{
    /// <summary>
    /// Abstraction for generating embeddings from text or structured data.
    /// </summary>
    public interface IEmbeddingProvider
    {
        /// <summary>
        /// Generate a single embedding vector from a text string
        /// </summary>
        /// <param name="text">Text to embed</param>
        /// <returns>Embedding vector as float array</returns>
        Task<float[]> GenerateEmbeddingAsync(string text);

        /// <summary>
        /// Generate multiple embedding vectors in batch
        /// </summary>
        /// <param name="texts">List of texts to embed</param>
        /// <returns>List of embedding vectors</returns>
        Task<List<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts);
    }
}
