﻿using Microsoft.Extensions.Logging;
using System.Text.Json;
using WesternStatesWater.WestDaat.Common.DataContracts;
using WesternStatesWater.WestDaat.Utilities;

namespace WesternStatesWater.WestDaat.Engines
{
    public sealed class GeoConnexEngine : EngineBase, IGeoConnexEngine
    {
        private readonly ITemplateResourceSdk _templateResourceSdk;

        public GeoConnexEngine(ITemplateResourceSdk rss, ILogger<GeoConnexEngine> logger) : base(logger)
        {
            _templateResourceSdk = rss;
        }

        string IGeoConnexEngine.BuildGeoConnexJson(Site site, Organization org)
        {
            var file = _templateResourceSdk.GetTemplate(Common.ResourceType.JsonLD);
            var geoConnexJson = string.Format(file,
                JsonEncode(
                    site.Longitude,                 // {0}
                    site.Latitude,                  // {1}
                    site.HUC8,                      // {2}
                    site.HUC12,                     // {3}
                    site.County,                    // {4}
                    site.SiteTypeCv,                // {5}
                    site.SiteUuid,                  // {6}
                    site.GniscodeCv,                // {7}
                    site.SiteName,                  // {8}
                    org.OrganizationDataMappingUrl, // {9}
                    site.Geometry?.ToString()       // {10}
                )
            );

            return geoConnexJson;
        }
        private string[] JsonEncode(params object[] args)
        {
            return args.Select(arg =>
                JsonEncodedText.Encode(arg?.ToString() ?? string.Empty).ToString()
            ).ToArray();
        }

    }
}