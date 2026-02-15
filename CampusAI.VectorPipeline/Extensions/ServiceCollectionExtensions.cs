using CampusAI.VectorPipeline.Configuration;
using CampusAI.VectorPipeline.Database.Factory;
using CampusAI.VectorPipeline.Embeddings;
using CampusAI.VectorPipeline.Services;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVectorPipeline(this IServiceCollection services)            
        {
            services.AddHttpClient<IEmbeddingProvider, OpenAIEmbeddingProvider>();
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<IEmbeddingProvider, OpenAIEmbeddingProvider>();
          
        }
    }
}
