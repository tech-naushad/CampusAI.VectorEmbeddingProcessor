using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.Modules.Campus.Entities
{
    public class CampusMain: IVectorizable
    {
        public int CampusID { get; set; }
        public string CampusName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string Country { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ContactNumber { get; set; } = "";
        public string Email { get; set; } = "";

        public Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "core",
                ["campusId"] = CampusID,
                ["city"] = City,
                ["country"] = Country,
                ["latitude"] = Latitude,
                ["longitude"] = Longitude
            };
        }

        public string GetTextToEmbed()
        {
            return $"""
                Campus: {CampusName}.
                Located at {Address}, {City}, {State}, {Country}.
                Contact number: {ContactNumber}.
                Email: {Email}.
                """;
        }

        public string GetVectorId()
        {
           return $"campus-{CampusID}";
        }
    }
}
