﻿using AutoMapper;
using System.Runtime.CompilerServices;
using WesternStatesWater.WestDaat.Common.DataContracts;
using EF = WesternStatesWater.WestDaat.Accessors.EntityFramework;

[assembly: InternalsVisibleTo("WesternStatesWater.WestDaat.Tests.AccessorTests")]

namespace WesternStatesWater.WestDaat.Accessors.Mapping
{
    internal class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<EF.AllocationAmountsFact, WaterRightDetails>()
                .ForMember(dest => dest.PriorityDate, opt => opt.MapFrom(source => source.AllocationPriorityDateID.HasValue ? source.AllocationPriorityDateNavigation.Date : (DateTime?)null))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(source => source.AllocationExpirationDateID.HasValue ? source.AllocationExpirationDateNavigation.Date : (DateTime?)null))
                .ForMember(dest => dest.BeneficialUse, opt => opt.MapFrom(source => source.PrimaryBeneficialUse.Name))
                .ForMember(dest => dest.AggregationInterval, opt => opt.MapFrom(source => source.VariableSpecific.AggregationInterval))
                .ForMember(dest => dest.AggregationIntervalUnitCv, opt => opt.MapFrom(source => source.VariableSpecific.AggregationIntervalUnitCv))
                .ForMember(dest => dest.AggregationStatisticCv, opt => opt.MapFrom(source => source.VariableSpecific.AggregationStatisticCv))
                .ForMember(dest => dest.AmountUnitCv, opt => opt.MapFrom(source => source.VariableSpecific.AmountUnitCv))
                .ForMember(dest => dest.ReportYearStartMonth, opt => opt.MapFrom(source => source.VariableSpecific.ReportYearStartMonth))
                .ForMember(dest => dest.ReportYearTypeCv, opt => opt.MapFrom(source => source.VariableSpecific.ReportYearTypeCv))
                .ForMember(dest => dest.VariableCv, opt => opt.MapFrom(source => source.VariableSpecific.VariableCv))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(source => source.Organization.OrganizationName))
                .ForMember(dest => dest.State, opt => opt.MapFrom(source => source.Organization.State))
                .ForMember(dest => dest.OrganizationContactName, opt => opt.MapFrom(source => source.Organization.OrganizationContactName))
                .ForMember(dest => dest.OrganizationContactEmail, opt => opt.MapFrom(source => source.Organization.OrganizationContactEmail))
                .ForMember(dest => dest.OrganizationPhoneNumber, opt => opt.MapFrom(source => source.Organization.OrganizationPhoneNumber))
                .ForMember(dest => dest.OrganizationWebsite, opt => opt.MapFrom(source => source.Organization.OrganizationWebsite));
            CreateMap<EF.SitesDim, Site>()
                .ForMember(a => a.AllocationIds, b => b.MapFrom(c => c.AllocationBridgeSitesFact.Select(allocation => allocation.AllocationBridgeId)))
                .ForMember(a => a.Geometry, b => b.MapFrom(c => c.Geometry ?? c.SitePoint));
            CreateMap<EF.SitesDim, SiteInfoListItem>();
            CreateMap<EF.OrganizationsDim, Organization>();
            CreateMap<EF.WaterSourcesDim, WaterSourceInfoListItem>();
        }
    }
}
