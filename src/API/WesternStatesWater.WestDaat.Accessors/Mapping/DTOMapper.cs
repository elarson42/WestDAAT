﻿using AutoMapper;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WesternStatesWater.WestDaat.Tests.AccessorTests")]
[assembly: InternalsVisibleTo("WesternStatesWater.WestDaat.Client.Functions")]

namespace WesternStatesWater.WestDaat.Accessors.Mapping
{
    internal static class DtoMapper
    {
        static IMapper _mapper;
        private static IConfigurationProvider _config;

        private static IMapper Mapper => _mapper ?? (_mapper = Configuration.CreateMapper());

        public static IConfigurationProvider Configuration
        {
            get
            {
                if (_config == null)
                {
                    var config = new AutoMapper.MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<ApiProfile>();                        
                    });
                    _config = config;
                }
                return _config;
            }
        }

        public static T Map<T>(this object source, Action<IMappingOperationOptions> opts = null)
        {
            return Mapper.Map<T>(source, opts ?? (a => { }));
        }

        public static void Map(this object source, object dest, Action<IMappingOperationOptions> opts = null)
        {
            Mapper.Map(source, dest, opts ?? (a => { }));
        }
    }
}
