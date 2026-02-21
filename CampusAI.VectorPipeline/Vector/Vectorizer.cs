using CampusAI.VectorPipeline.Embeddings;

namespace CampusAI.VectorPipeline.Vector
{
    public static class Vectorizer
    {
        public static async Task<List<VectorRecord>> ToVectorRecordsBatchAsync<T>(
            this IEnumerable<T> entities,
            IEmbeddingProvider embeddingProvider) where T : IVectorizable
        {
            var entityList = entities.ToList();

            // Get all texts first
            var texts = entityList.Select(e => e.GetVectorId()).ToList();

            // Single batch call — one HTTP request for all
            var embeddings = await embeddingProvider.GenerateBatchEmbeddingsAsync(texts);

            // Zip entities with their corresponding embedding by index
            return entityList
                .Zip(embeddings, (entity, embedding) => new VectorRecord
                {
                    Id = entity.GetVectorId(),
                    Values = embedding,
                    Metadata = entity.GetMetadata()
                })
                .ToList();
        }
        public static async Task<VectorRecord> ToVectorRecordsAsync(
            this IVectorizable entity,
            IEmbeddingProvider embeddingProvider)
        {
            var embedding = await embeddingProvider.GenerateEmbeddingAsync(entity.GetTextToEmbed());

            return new VectorRecord
            {
                Id = entity.GetVectorId(),
                Values = embedding,
                Metadata = entity.GetMetadata()
            };
        }
         

        public static async Task<List<VectorRecord>> ToVectorRecordsAsync<T>(
            this IEnumerable<T> entities,
            IEmbeddingProvider embeddingProvider) where T : IVectorizable
        {
            var list = new List<VectorRecord>();
            foreach (var entity in entities)
            {
                list.Add(await entity.ToVectorRecordsAsync(embeddingProvider));
            }
            return list;
        }
    }

}
