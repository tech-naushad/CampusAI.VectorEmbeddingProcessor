using CampusAI.Modules.Campus.Entities;
using CampusAI.Modules.Campus.Repositories;
using CampusAI.Modules.Campus.Services;
using CampusAI.Modules.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace CampusAI.Ingestion.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IngestionController : ControllerBase
    {
        private readonly ICampusIngestionService _iService;
        ICampusIngestionRepository _ingestionRepository;
        public IngestionController(ICampusIngestionService iService)
        {
            //_ingestionOrchestrator = ingestionOrchestrator;
            _iService = iService;
        }

        [HttpPost]
        public async Task<IActionResult> RunPipelineAsync([FromBody] ModuleOptions request)
        {
            var result = await _iService.RunAsync();
            // 1. Fetch from SQL
           // var rows = await _ingestionRepository.BuildVectorRecords(result.ToList());

            // 2. Build vector records
            //var records = _builder.Build(rows);

            //_ingestionOrchestrator.RunModuleAsync(
            //    repo: null, // TODO: inject and pass the actual repository
            //    moduleName: "ExampleModule",
            //    toTextFunc: row => row.ToString(), // TODO: provide actual conversion logic
            //    indexName: "example-index");
            return Ok();
        }
    }
}
