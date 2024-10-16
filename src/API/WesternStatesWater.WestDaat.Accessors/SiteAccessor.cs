﻿using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WestDaat.Accessors.EntityFramework;
using WesternStatesWater.WestDaat.Accessors.Mapping;
using WesternStatesWater.WestDaat.Common.DataContracts;

namespace WesternStatesWater.WestDaat.Accessors
{
    internal class SiteAccessor : AccessorBase, ISiteAccessor
    {
        public SiteAccessor(ILogger<SiteAccessor> logger, IDatabaseContextFactory databaseContextFactory) : base(logger)
        {
            _databaseContextFactory = databaseContextFactory;
        }

        private readonly IDatabaseContextFactory _databaseContextFactory;

        async Task<Site> ISiteAccessor.GetSiteByUuid(string siteUuid)
        {
            await using var db = _databaseContextFactory.Create();
            return await db.SitesDim
                .Where(x => x.SiteUuid == siteUuid)
                .ProjectTo<Site>(DtoMapper.Configuration)
                .SingleAsync();
        }

        async Task<SiteDigest> ISiteAccessor.GetSiteDigestByUuid(string siteUuid)
        {
            await using var db = _databaseContextFactory.Create();
            return await db.SitesDim
                .Where(x => x.SiteUuid == siteUuid)
                .ProjectTo<SiteDigest>(DtoMapper.Configuration)
                .SingleAsync();
        }

        async Task<List<Site>> ISiteAccessor.GetSites()
        {
            var db = _databaseContextFactory.Create();
            db.Database.SetCommandTimeout(int.MaxValue);
            return await db.SitesDim
                .ProjectTo<Site>(DtoMapper.Configuration)
                .ToListAsync();
        }

        public async Task<SiteDetails> GetSiteDetailsByUuid(string siteUuid)
        {
            await using var db = _databaseContextFactory.Create();
            return await db.SitesDim
                .Where(x => x.SiteUuid == siteUuid)
                .ProjectTo<SiteDetails>(DtoMapper.Configuration)
                .SingleAsync();
        }

        async Task<SiteLocation> ISiteAccessor.GetWaterSiteLocationByUuid(string siteUuid)
        {
            await using var db = _databaseContextFactory.Create();
            return await db.SitesDim.Where(x => x.SiteUuid == siteUuid)
                .ProjectTo<SiteLocation>(DtoMapper.Configuration)
                .SingleAsync();
        }

        public async Task<List<WaterSourceInfoListItem>> GetWaterSiteSourceInfoListByUuid(string siteUuid)
        {
            await using var db = _databaseContextFactory.Create();
            return await db.SitesDim.Where(x => x.SiteUuid == siteUuid)
                .SelectMany(x => x.WaterSourceBridgeSitesFact
                    .Select(a => a.WaterSource))
                .ProjectTo<WaterSourceInfoListItem>(DtoMapper.Configuration)
                .ToListAsync();
        }

        public async Task<List<WaterRightInfoListItem>> GetWaterRightInfoListByUuid(string siteUuid)
        {
            await using var db = _databaseContextFactory.Create();
            return await db.AllocationAmountsFact
                .Where(x => x.AllocationBridgeSitesFact.Any(y => y.Site.SiteUuid == siteUuid))
                .ProjectTo<WaterRightInfoListItem>(DtoMapper.Configuration)
                .ToListAsync();
        }

        public IEnumerable<GeoConnex> GetJSONLDData()
        {
            var db = _databaseContextFactory.Create();
            var query = db.SitesDim.Select(a => new GeoConnex
            {
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                SiteTypeCv = a.SiteTypeCv,
                SiteUuid = a.SiteUuid,
                SiteName = a.SiteName,
            });

            return query;
        }
    }
}