using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.Configuration
{
    public class PineconeOptions
    {
        public string ApiKey { get; set; }
        public string Environment { get; set; }
        public string IndexName { get; set; }
        public string Namespace { get; set; }
    }
}
