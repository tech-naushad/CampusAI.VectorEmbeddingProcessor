using CampusAI.Modules.Campus.Repositories;
using CampusAI.VectorPipeline.Embeddings;
using CampusAI.VectorPipeline.Services;
using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var stopwatch = Stopwatch.StartNew();
            // Vectorize each module
            //var campusVectors = await campuses.ToVectorRecordsAsync(_embeddingProvider);
            //var hoursVectors = await hours.ToVectorRecordsAsync(_embeddingProvider);
            //var directionsVectors = await directions.ToVectorRecordsAsync(_embeddingProvider);
            //var transportVectors = await transport.ToVectorRecordsAsync(_embeddingProvider);

            var campusTask = campuses.ToVectorRecordsBatchAsync(_embeddingProvider);
            var hoursTask = hours.ToVectorRecordsBatchAsync(_embeddingProvider);
            var directionsTask = directions.ToVectorRecordsBatchAsync(_embeddingProvider);
            var transportTask = transport.ToVectorRecordsBatchAsync(_embeddingProvider);

            await Task.WhenAll(campusTask, hoursTask, directionsTask, transportTask);

            var campusVectors = await campusTask;
            var hoursVectors = await hoursTask;
            var directionsVectors = await directionsTask;
            var transportVectors = await transportTask;

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds / 1000;

            // Upsert to Pinecone
            //await _pineconeService.UpsertAsync(campusVectors, "campus");
            //await _pineconeService.UpsertAsync(hoursVectors, "opening-hours");
            //await _pineconeService.UpsertAsync(directionsVectors, "directions");
            //await _pineconeService.UpsertAsync(transportVectors, "transport");

            await Task.WhenAll(
                _pineconeService.UpsertAsync(campusVectors, "campus"),
                _pineconeService.UpsertAsync(hoursVectors, "opening-hours"),
                _pineconeService.UpsertAsync(directionsVectors, "directions"),
                _pineconeService.UpsertAsync(transportVectors, "transport")
            );

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
