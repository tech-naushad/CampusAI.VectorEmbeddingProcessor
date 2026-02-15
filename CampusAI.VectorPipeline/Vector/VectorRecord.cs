using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.Vector
{
    public class VectorRecord
    {
        public string Id { get; set; }
        public float[] Values { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
