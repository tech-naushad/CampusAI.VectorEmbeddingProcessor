using CampusAI.Modules.Campus.Entities;
using CampusAI.Modules.Campus.Repositories;
using CampusAI.Modules.Campus.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AdModuleServices(this IServiceCollection services)
        { 
            services.AddScoped<ICampusIngestionRepository, CampusIngestionRepository>();
            services.AddScoped<ICampusIngestionService, CampusIngestionService>();
            
        }
    }
}
