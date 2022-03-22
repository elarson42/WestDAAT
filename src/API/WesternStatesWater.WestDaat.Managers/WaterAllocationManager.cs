using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using GeoJSON.Text.Feature;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WestDaat.Accessors;
using WesternStatesWater.WestDaat.Common.DataContracts;
using WesternStatesWater.WestDaat.Common.Exceptions;
using WesternStatesWater.WestDaat.Contracts.Client;
using WesternStatesWater.WestDaat.Engines;
using WesternStatesWater.WestDaat.Managers.Mapping;

namespace WesternStatesWater.WestDaat.Managers
{
    public sealed class WaterAllocationManager : ManagerBase, IWaterAllocationManager
    {
        private readonly IGeoConnexEngine _geoConnexEngine;
        private readonly ISiteAccessor _siteAccessor;
        private readonly IWaterAllocationAccessor _waterAllocationAccessor;
        private readonly INldiAccessor _nldiAccessor;

        public WaterAllocationManager(
            INldiAccessor nldiAccessor,
            ISiteAccessor siteAccessor,
            IWaterAllocationAccessor waterAllocationAccessor,
            IGeoConnexEngine geoConnexEngine,
            ILogger<WaterAllocationManager> logger) : base(logger)
        {
            _nldiAccessor = nldiAccessor;
            _siteAccessor = siteAccessor;
            _waterAllocationAccessor = waterAllocationAccessor;
            _geoConnexEngine = geoConnexEngine;
        }

        async Task<string> IWaterAllocationManager.GetWaterAllocationSiteGeoconnexIntegrationData(string siteUuid)
        {
            var site = await _siteAccessor.GetSiteByUuid(siteUuid);
            if (site.AllocationIds == null || !site.AllocationIds.Any())
            {
                throw new WestDaatException($"No AllocationAmounts found for site uuid [{siteUuid}]");
            }

            var organization = _waterAllocationAccessor.GetWaterAllocationAmountOrganizationById(site.AllocationIds.First());
            var json = _geoConnexEngine.BuildGeoConnexJson(site, organization);

            return json;
        }

        async Task<FeatureCollection> IWaterAllocationManager.GetNldiFeatures(double latitude, double longitude, NldiDirections directions, NldiDataPoints dataPoints)
        {
            return await _nldiAccessor.GetNldiFeatures(latitude, longitude, directions, dataPoints);
        }

        public async Task<SiteDetails> GetSiteDetails(string siteUuid)
        {
            return await _siteAccessor.GetSiteDetailsByUuid(siteUuid);
        }

        public async Task<WaterRightDetails> GetWaterRightDetails(long waterRightsId)
        {
            return await _waterAllocationAccessor.GetWaterRightDetailsById(waterRightsId);
        }

        public async Task<List<SiteInfoListItem>> GetWaterRightSiteInfoList(long waterRightsId)
        {
            return await _waterAllocationAccessor.GetWaterRightSiteInfoById(waterRightsId);
        }

        public async Task<List<WaterSourceInfoListItem>> GetWaterRightSourceInfoList(long waterRightsId)
        {
            return await _waterAllocationAccessor.GetWaterRightSourceInfoById(waterRightsId);
        }

        public async Task<List<SiteLocation>> GetWaterRightSiteLocations(long waterRightsId)
        {
            return await _waterAllocationAccessor.GetWaterRightSiteLocationsById(waterRightsId);
        }
    }
}
