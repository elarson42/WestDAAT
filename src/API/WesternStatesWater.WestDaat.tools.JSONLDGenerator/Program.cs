﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;
using WesternStatesWater.WestDaat.Accessors;
using WesternStatesWater.WestDaat.Common.Configuration;
using WesternStatesWater.WestDaat.Common.DataContracts;
using WesternStatesWater.WestDaat.Utilities;

namespace WesternStatesWater.WestDaat.Tools.JSONLDGenerator
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("personal.settings.json", optional: true, reloadOnChange: true)
                        .Build();

                var hostBuilder = Host.CreateDefaultBuilder();
                var services = hostBuilder.ConfigureServices((_, services) =>
                {
                    services.AddScoped(_ => config.GetDatabaseConfiguration());
                    services.AddScoped(_ => config.GetBlobStorageConfiguration());
                    services.AddScoped(_ => config.GetPerformanceConfiguration());
                    services.AddTransient<Accessors.EntityFramework.IDatabaseContextFactory, Accessors.EntityFramework.DatabaseContextFactory>();
                    services.AddScoped<IWaterAllocationAccessor, WaterAllocationAccessor>();
                    services.AddScoped<IBlobStorageSdk, BlobStorageSdk>();
                    services.AddScoped<ITemplateResourceSdk, TemplateResourceSdk>();
                    services.BuildServiceProvider();
                }).Build();

                var waterAllocationAccessor = services.Services.GetService<IWaterAllocationAccessor>();
                if (waterAllocationAccessor == null)
                {
                    Console.WriteLine("WaterAllocationAccessor service was null");
                    return;
                }

                var templateResourceSdk = services.Services.GetService<ITemplateResourceSdk>();
                if (templateResourceSdk == null)
                {
                    Console.WriteLine("TemplateSdk was null");
                    return;
                }

                var blobStorageSdk = services.Services.GetService<IBlobStorageSdk>();
                if (blobStorageSdk == null)
                {
                    Console.WriteLine("Blob storage service was null");
                    return;
                }
                var stringFile = templateResourceSdk.GetTemplate(Common.ResourceType.JsonLD);

                var rawDataEnumerable = waterAllocationAccessor.GetJSONLDData();

                var blobStream = await blobStorageSdk.GetBlobStream("$web", "jsonld.json", true);
                using (var sw = new StreamWriter(blobStream))
                {
                    var hasJsonBeenStreamed = false;
                    await sw.WriteLineAsync("[");
                    foreach (var site in rawDataEnumerable)
                    {
                        var file = BuildGeoConnexJson(stringFile, site);
                        if (!string.IsNullOrEmpty(file))
                        {
                            if (hasJsonBeenStreamed)
                            {
                                sw.WriteLine(",");
                            }
                            await sw.WriteLineAsync(file);
                            hasJsonBeenStreamed = true;
                        }
                    };
                    await sw.WriteLineAsync("]");
                    await sw.FlushAsync();
                    sw.Close();
                }
                Console.WriteLine("Finish");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private static string BuildGeoConnexJson(string stringFile, GeoConnex geoConnex)
        {
            if (geoConnex != null)
            {
                var geoConnexJson = string.Format(stringFile, 
                    JsonEncode(
                        geoConnex.Longitude ?? default,                  // {0}
                        geoConnex.Latitude ?? default,                   // {1}
                        geoConnex.SiteTypeCv,                            // {2}
                        geoConnex.SiteUuid,                              // {3}
                        geoConnex.GniscodeCv,                            // {4}
                        geoConnex.SiteName,                              // {5}
                        geoConnex.OrganizationDataMappingUrl,            // {6}
                        geoConnex.Geometry?.ToString() ?? string.Empty   // {7}
                    ));

                return geoConnexJson;
            }

            return string.Empty;
        }

        private static string[] JsonEncode(params object[] args)
        {
            return args.Select(arg =>
                JsonEncodedText.Encode(arg?.ToString() ?? string.Empty).ToString()
            ).ToArray();
        }
    }
}
