using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WestDaat.Common.DataContracts;
using WesternStatesWater.WestDaat.Contracts.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WesternStatesWater.WestDaat.Client.Functions
{
    public class WaterAllocation : FunctionBase
    {
        public WaterAllocation(IWaterAllocationManager waterAllocationManager, ILogger<WaterAllocation> logger)
        {
            _waterAllocationManager = waterAllocationManager;
            _logger = logger;
        }

        private readonly IWaterAllocationManager _waterAllocationManager;
        private readonly ILogger _logger;

        [FunctionName(nameof(NldiFeatures))]
        public async Task<IActionResult> NldiFeatures([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "NldiFeatures/@{latitude},{longitude}")] HttpRequest req, double latitude, double longitude)
        {
            _logger.LogInformation("Getting NLDI Features []");

            var directions = Enum.Parse<NldiDirections>(req.Query["dir"]);
            var dataPoints = Enum.Parse<NldiDataPoints>(req.Query["points"]);

            var results = await _waterAllocationManager.GetNldiFeatures(latitude, longitude, directions, dataPoints);

            return JsonResult(results);
        }

        [FunctionName(nameof(GetWaterAllocationSiteDetails)), AllowAnonymous]
        public async Task<IActionResult> GetWaterAllocationSiteDetails([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = nameof(GetWaterAllocationSiteDetails))] HttpRequest request)
        {
            var siteUuid = await JsonSerializer.DeserializeAsync<string>(request.Body);

            var result = _waterAllocationManager.GetWaterAllocationSiteGeoconnexIntegrationData(siteUuid);

            return new OkObjectResult(result);
        }

        [FunctionName(nameof(GenerateAllocationGeoJson))]
        public async Task<IActionResult> GenerateAllocationGeoJson([HttpTrigger("post", Route = nameof(GenerateAllocationGeoJson))] HttpRequest request)
        {
            var result = await _waterAllocationManager.GetWaterAllocationAmountsGeoJson();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Allocations.geojson");
            await File.WriteAllTextAsync(filePath, result);

            return new OkObjectResult($"Success - geojson file writen to {filePath}. JSON: {result}");
        }
    }
}
