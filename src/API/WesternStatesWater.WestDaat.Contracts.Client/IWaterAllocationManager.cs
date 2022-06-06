using GeoJSON.Text.Feature;
using WesternStatesWater.WestDaat.Common;

namespace WesternStatesWater.WestDaat.Contracts.Client
{
    public interface IWaterAllocationManager : IServiceContractBase
    {
        Task<string> GetWaterAllocationSiteGeoconnexIntegrationData(string siteUuid);
        
        Task<FeatureCollection> GetNldiFeatures(double latitude, double longitude, NldiDirections directions, NldiDataPoints dataPoints);

        Task<SiteDetails> GetSiteDetails(string siteUuid);

        Task<Feature> GetWaterSiteLocation(string siteUuid);

        Task<WaterRightDetails> GetWaterRightDetails(long waterRightsId);

        Task<List<SiteInfoListItem>> GetWaterRightSiteInfoList(long waterRightsId);

        Task<List<WaterSourceInfoListItem>> GetWaterRightSourceInfoList(long waterRightsId);

        Task<FeatureCollection> GetWaterRightSiteLocations(long waterRightsId);

        Task<List<WaterRightsDigest>> GetWaterRightsDigestsBySite(string siteUuid);
    }
}
