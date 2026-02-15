using CampusAI.VectorPipeline.Embeddings;

namespace CampusAI.VectorPipeline.Vector
{
    public static class Vectorizer
    {
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
