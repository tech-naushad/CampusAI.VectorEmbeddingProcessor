using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.Modules.Campus.Entities
{
    public class CampusDirections: IVectorizable
    {
        public int CampusID { get; set; }
        public string FromLocation { get; set; } = "";
        public int TravelTimeMinutes { get; set; }
        public string DirectionsText { get; set; } = "";
        public string TransportMode { get; set; } = "";

        public Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "directions",
                ["campusId"] = CampusID,
                ["fromLocation"] = FromLocation,
                ["transportMode"] = TransportMode,
                ["travelTimeMinutes"] = TravelTimeMinutes
            };
        }

        public string GetTextToEmbed()
        {
            return $"""
            Directions from {FromLocation} to campus {CampusID}.
            Mode of transport: {TransportMode}.
            Estimated travel time: {TravelTimeMinutes} minutes.
            Directions details: {DirectionsText}
            """;
        }

        public string GetVectorId()
        {
            return $"campus-{CampusID}-from-{FromLocation.Replace(" ", "-").ToLower()}"; 
        }
    }
}
