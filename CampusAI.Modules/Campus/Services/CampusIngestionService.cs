using CampusAI.Modules.Campus.Repositories;
using CampusAI.VectorPipeline.Embeddings;
using CampusAI.VectorPipeline.Services;
using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.Modules.Campus.Services
{
    public interface ICampusIngestionService
    {
        Task<Dictionary<string, int>> RunAsync();
    }
    public class CampusIngestionService : ICampusIngestionService
    {
        private readonly ICampusIngestionRepository _repository;
        private readonly IEmbeddingProvider _embeddingProvider;
        private readonly IPineconeService _pineconeService;

        public CampusIngestionService(ICampusIngestionRepository repository,
            IEmbeddingProvider embeddingProvider, IPineconeService pineconeService)
        {
            _repository = repository;
            _embeddingProvider = embeddingProvider;
            _pineconeService = pineconeService;
        }
        public async Task<Dictionary<string, int>> RunAsync()
        {
            // Fetch all module data
            var (campuses, hours, directions, transport) = await _repository.FetchAsync();

            // Vectorize each module
            var campusVectors = await campuses.ToVectorRecordsAsync(_embeddingProvider);
            var hoursVectors = await hours.ToVectorRecordsAsync(_embeddingProvider);
            var directionsVectors = await directions.ToVectorRecordsAsync(_embeddingProvider);
            var transportVectors = await transport.ToVectorRecordsAsync(_embeddingProvider);

            // Upsert to Pinecone
            await _pineconeService.UpsertAsync(campusVectors,"");
            await _pineconeService.UpsertAsync(hoursVectors, "","");
            await _pineconeService.UpsertAsync(directionsVectors, "");
            await _pineconeService.UpsertAsync(transportVectors, "");

            // Return a clean response
            return new Dictionary<string, int>
            {
                ["campuses"] = campusVectors.Count,
                ["hours"] = hoursVectors.Count,
                ["directions"] = directionsVectors.Count,
                ["transport"] = transportVectors.Count
            };
        }
    }
}
