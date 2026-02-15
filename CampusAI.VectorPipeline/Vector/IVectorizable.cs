using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.VectorPipeline.Vector
{
    public interface IVectorizable
    {
        string GetTextToEmbed();
        Dictionary<string, object> GetMetadata();
        string GetVectorId();
    }
}
