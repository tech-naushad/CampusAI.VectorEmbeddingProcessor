using CampusAI.Modules.Campus.Entities;
using CampusAI.VectorPipeline.Database.Factory;
using Dapper;
using System.Data;


namespace CampusAI.Modules.Campus.Repositories
{
    public class CampusIngestionRepository : ICampusIngestionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CampusIngestionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(List<CampusMain> campuses,
                      List<CampusOpeningHours> hours,
                      List<CampusDirections> directions,
                      List<CampusTransportation> transport)> FetchAsync(DateTime? lastModified = null)
        {
            using var conn = _connectionFactory.CreateConnection(DatabaseType.CampusDb);

            // Call the stored procedure
            using var multi = await conn.QueryMultipleAsync(
                "dbo.sp_GetCampusIngestionData",
                commandType: CommandType.StoredProcedure);

            var campuses = (await multi.ReadAsync<CampusMain>()).ToList();
            var hours = (await multi.ReadAsync<CampusOpeningHours>()).ToList();
            var directions = (await multi.ReadAsync<CampusDirections>()).ToList();
            var transport = (await multi.ReadAsync<CampusTransportation>()).ToList();

            return (campuses, hours, directions, transport);
        }


        //public Task<List<VectorRecord>> BuildVectorRecords(List<CampusIngestionEntity> rows)
        //{
        //    var result = rows.Select(row => new VectorRecord
        //    {
        //        Id = $"campus-{row.CampusID}-day{row.DayOfWeek}-{Slugify(row.FromLocation)}",

        //        Values = Array.Empty<float>(),

        //        Metadata = new Dictionary<string, object>
        //        {
        //            // Identity
        //            ["campusId"] = row.CampusID,
        //            ["campusName"] = row.CampusName,
        //            // Location
        //            ["city"] = row.City,
        //            ["country"] = row.Country,
        //            ["latitude"] = row.Latitude,
        //            ["longitude"] = row.Longitude,
        //            // Contact
        //            ["contactNumber"] = row.ContactNumber,
        //            ["email"] = row.Email,
        //            // Hours
        //            ["dayOfWeek"] = row.DayOfWeek,
        //            ["openTime"] = row.OpenTime,
        //            ["closeTime"] = row.CloseTime,
        //            ["isClosed"] = row.IsClosed,
        //            // Directions
        //            ["fromLocation"] = row.FromLocation,
        //            ["transportMode"] = row.TransportMode,
        //            ["travelTimeMinutes"] = row.TravelTimeMinutes,
        //            // Transport
        //            ["transportType"] = row.TransportType,
        //            ["routeOrLocation"] = row.RouteOrLocation,
        //            ["schedule"] = row.Schedule,
        //            ["capacity"] = row.Capacity
        //        }
        //    }).ToList();
        //    return Task.FromResult(result);
        //}


        //private string Slugify(string input)
        //{
        //    input = string.IsNullOrEmpty(input) ? "na" :
        //    input.ToLower().Replace(" ", "-");
        //    return input;
        //}
    }

}
