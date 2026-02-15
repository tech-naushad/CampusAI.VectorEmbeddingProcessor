using CampusAI.VectorPipeline.Vector;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.Modules.Campus.Entities
{
    public class CampusOpeningHours: IVectorizable
    {
        public int CampusID { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public int DayOfWeek { get; set; }
        public bool IsClosed { get; set; }
        public string CampusName { get; set; } = ""; // Needed for text context

        public Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "hours",
                ["campusId"] = CampusID,
                ["dayOfWeek"] = DayOfWeek,
                ["isClosed"] = IsClosed
            };
        }

        public string GetTextToEmbed()
        {
           return $"{CampusName} is {(IsClosed ? "closed" : $"open from {OpenTime} to {CloseTime}")} on {DayOfWeek}.";
        }

        public string GetVectorId()
        {
            return $"campus-{CampusID}-hours-{DayOfWeek}"; 
        }
    }
}
