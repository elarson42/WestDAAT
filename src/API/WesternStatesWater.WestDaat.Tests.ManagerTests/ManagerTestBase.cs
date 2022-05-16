﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WesternStatesWater.WestDaat.Common.Configuration;

namespace WesternStatesWater.WestDaat.Tests.ManagerTests
{
    public abstract class ManagerTestBase
    {
        private ILoggerFactory _loggerFactory;
        public static IConfiguration Configuration { get; private set; }

        static ManagerTestBase()
        {
            Configuration = new ConfigurationBuilder()
                                        .AddInMemoryCollection(DefaultTestConfiguration)
                                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                        .AddJsonFile("personal.settings.json", optional: true, reloadOnChange: true)
                                        .AddEnvironmentVariables()
                                        .Build();
        }

        public static Dictionary<string, string> DefaultTestConfiguration => new()
        {
            { $"{ConfigurationRootNames.Smtp}:{nameof(EmailServiceConfiguration.FeedbackFrom)}", "test@test.com" },
            { $"{ConfigurationRootNames.Smtp}:{nameof(EmailServiceConfiguration.FeedbackTo)}:01", "test01@test.com" },
            { $"{ConfigurationRootNames.Smtp}:{nameof(EmailServiceConfiguration.FeedbackTo)}:02", "test02@test.com" }
        };

        [TestInitialize]
        public virtual void BaseTestInitialize()
        {
            var services = new ServiceCollection()
               .AddLogging(config => config.AddConsole())
               .BuildServiceProvider();

            _loggerFactory = services.GetRequiredService<ILoggerFactory>();
        }

        [TestCleanup]
        public void BaseTestCleanup()
        {
            _loggerFactory.Dispose();
        }

        public ILogger<T> CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }
    }
}
