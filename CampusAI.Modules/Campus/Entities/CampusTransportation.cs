using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.Modules.Campus.Entities
{
    public class CampusTransportation: IVectorizable
    {
        public int CampusID { get; set; }
        public string TransportType { get; set; } = "";
        public string RouteOrLocation { get; set; } = "";
        public int Capacity { get; set; }
        public string Schedule { get; set; } = "";
        public string Notes { get; set; } = "";

        // Unique vector ID for Pinecone
        public string GetVectorId()
        {
            // Replace spaces with dash to avoid invalid characters
            return $"campus-{CampusID}-transport-{TransportType.Replace(" ", "-").ToLower()}-{RouteOrLocation.Replace(" ", "-").ToLower()}";
        }

        // Text to embed
        public string GetTextToEmbed()
        {
            return $"""
            Transport Type: {TransportType}.
            Route/Location: {RouteOrLocation}.
            Capacity: {Capacity}.
            Schedule: {Schedule}.
            Notes: {Notes}.
            """;
        }

        // Metadata for filtering in Pinecone
        public Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "transport",
                ["campusId"] = CampusID,
                ["transportType"] = TransportType,
                ["routeOrLocation"] = RouteOrLocation,
                ["capacity"] = Capacity,
                ["schedule"] = Schedule
            };
        }
    }
}
