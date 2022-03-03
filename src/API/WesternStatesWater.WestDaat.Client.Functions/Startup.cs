﻿using WesternStatesWater.WestDaat.Accessors;
﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WesternStatesWater.WestDaat.Accessors;
using WesternStatesWater.WestDaat.Common.Configuration;
using WesternStatesWater.WestDaat.Engines;
using WesternStatesWater.WestDaat.Managers;
using WesternStatesWater.WestDaat.Utilities;

[assembly: FunctionsStartup(typeof(WesternStatesWater.WestDaat.Client.Functions.Startup))]

namespace WesternStatesWater.WestDaat.Client.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.SetBasePath(Environment.CurrentDirectory)
                                        .AddInMemoryCollection(ConfigurationHelper.DefaultConfiguration)
                                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                        .AddEnvironmentVariables();
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.AddSingleton(configuration.GetDatabaseConfiguration());
            builder.Services.AddSingleton(configuration.GetNldiConfiguration());

            builder.Services.AddTransient<ITestManager, TestManager>();

            builder.Services.AddTransient<ITestEngine, TestEngine>();

            builder.Services.AddTransient<ITestAccessor, TestAccessor>();
            builder.Services.AddLogging(logging =>
            {
                logging.AddApplicationInsights();
                logging.AddConsole();
            });
        }
    }
}
