﻿using GeoJSON.Text.Feature;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using WesternStatesWater.WestDaat.Common;
using WesternStatesWater.WestDaat.Common.Exceptions;

namespace WesternStatesWater.WestDaat.Utilities
{
    public class UsgsNldiSdk : IUsgsNldiSdk
    {
        public UsgsNldiSdk(HttpClient httpClient, ILogger<UsgsNldiSdk> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly Dictionary<NavigationMode, string> _navigationModeStrings = new Dictionary<NavigationMode, string>()
        {
            { NavigationMode.UpstreamMain, "UM" },
            { NavigationMode.UpstreamTributaries, "UT" },
            { NavigationMode.DownstreamMain, "DM" },
            { NavigationMode.DownstreamDiversions, "DD" },
        };

        public async Task<FeatureCollection> GetFeatureByCoordinates(double latitude, double longitude)
        {
            using (new TimerLogger($"Getting features by coordinates [{latitude}] [{longitude}]", _logger))
            {
                var query = new QueryBuilder();
                query.Add("coords", $"POINT({longitude} {latitude})");
                var response = await _httpClient.GetAsync(new Uri($"linked-data/comid/position{query}", UriKind.Relative));
                return await ProcessFeatureCollectionResponse(response);
            }
        }

        public async Task<FeatureCollection> GetFlowlines(string comid, NavigationMode navigationMode, int distanceInKm)
        {
            using (new TimerLogger($"Getting flowlines [{comid}] [{navigationMode}] [{distanceInKm}]", _logger))
            {
                var query = new QueryBuilder();
                query.Add("distance", distanceInKm.ToString());
                var response = await _httpClient.GetAsync(new Uri($"linked-data/comid/{comid}/navigation/{_navigationModeStrings[navigationMode]}/flowlines{query}", UriKind.Relative));
                return await ProcessFeatureCollectionResponse(response);
            }
        }

        private async Task<FeatureCollection> ProcessFeatureCollectionResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new WestDaatException($"Invalid NLDI Response Code [{(int)response.StatusCode} - {response.ReasonPhrase}] [{await response.Content.ReadAsStringAsync()}]");
            }
            try
            {
                return await JsonSerializer.DeserializeAsync<FeatureCollection>(await response.Content.ReadAsStreamAsync());
            }
            catch (Exception)
            {
                _logger.LogError($"Error deserializing NLDI Response [{await response.Content.ReadAsStringAsync()}]");
                throw;
            }
        }
    }

    public enum NavigationMode
    {
        UpstreamMain,
        UpstreamTributaries,
        DownstreamMain,
        DownstreamDiversions
    }
}