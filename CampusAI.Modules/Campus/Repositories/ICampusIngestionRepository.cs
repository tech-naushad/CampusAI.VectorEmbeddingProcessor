using CampusAI.Modules.Campus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CampusAI.Modules.Campus.Repositories
{
    public interface ICampusIngestionRepository
    {
        Task<(List<CampusMain> campuses,
                      List<CampusOpeningHours> hours,
                      List<CampusDirections> directions,
                      List<CampusTransportation> transport)> FetchAsync(DateTime? lastModified = null);
    }
}
