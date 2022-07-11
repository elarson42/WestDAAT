﻿using GeoJSON.Text.Feature;
using WesternStatesWater.WestDaat.Accessors;
using WesternStatesWater.WestDaat.Common.Exceptions;
using WesternStatesWater.WestDaat.Contracts.Client;
using WesternStatesWater.WestDaat.Engines;
using WesternStatesWater.WestDaat.Managers;
using CommonContracts = WesternStatesWater.WestDaat.Common.DataContracts;
using CsvModels = WesternStatesWater.WestDaat.Accessors.CsvModels;
using NetTopologySuite.Geometries;
using WesternStatesWater.WestDaat.Utilities;
using WesternStatesWater.WestDaat.Common.Constants;
using WesternStatesWater.WestDaat.Common.Constants.RiverBasins;
using WesternStatesWater.WestDaat.Common.Configuration;
using System.IO;

namespace WesternStatesWater.WestDaat.Tests.ManagerTests
{
    [TestClass]
    public class WaterAllocationManagerTests : ManagerTestBase
    {
        private readonly Mock<INldiAccessor> _nldiAccessorMock = new(MockBehavior.Strict);
        private readonly Mock<IGeoConnexEngine> _geoConnexEngineMock = new Mock<IGeoConnexEngine>(MockBehavior.Strict);
        private readonly Mock<ILocationEngine> _locationEngineMock = new Mock<ILocationEngine>(MockBehavior.Strict);
        private readonly Mock<ISiteAccessor> _siteAccessorMock = new Mock<ISiteAccessor>(MockBehavior.Strict);
        private readonly Mock<IWaterAllocationAccessor> _waterAllocationAccessorMock = new Mock<IWaterAllocationAccessor>(MockBehavior.Strict);
        private readonly Mock<ITemplateResourceSdk> _templateResourceSdk = new Mock<ITemplateResourceSdk>(MockBehavior.Strict);
        private readonly Mock<PerformanceConfiguration> _performanceConfiguration = new Mock<PerformanceConfiguration>(MockBehavior.Strict);

        private readonly string _citationFile = Resources.resources.citation;

        [TestMethod]
        public async Task GeoConnexEngine_GetWaterAllocationSiteGeoconnexIntegrationData_ShouldCallEngine()
        {
            // ARRANGE 
            _geoConnexEngineMock.Setup(x => x.BuildGeoConnexJson(It.IsAny<CommonContracts.Site>(), It.IsAny<CommonContracts.Organization>())).Returns("{Foo: \"bar\"}");
            _siteAccessorMock.Setup(x => x.GetSiteByUuid(It.IsAny<string>())).ReturnsAsync(new CommonContracts.Site
            {
                AllocationIds = new List<long> { 1, 2, 3 }
            });
            _waterAllocationAccessorMock.Setup(x => x.GetWaterAllocationAmountOrganizationById(It.IsAny<long>())).Returns(new CommonContracts.Organization());

            var manager = CreateWaterAllocationManager();

            // ACT 
            var response = await manager.GetWaterAllocationSiteGeoconnexIntegrationData("test");

            // ASSERT 
            response.Should().NotBeNull();
            _geoConnexEngineMock.Verify(t =>
                t.BuildGeoConnexJson(It.IsAny<CommonContracts.Site>(), It.IsAny<CommonContracts.Organization>()),
                Times.Once()
            );
        }

        [TestMethod]
        public async Task GeoConnexEngine_GetWaterAllocationSiteGeoconnexIntegrationData_MissingAllocations()
        {
            // ARRANGE 
            _siteAccessorMock.Setup(x => x.GetSiteByUuid(It.IsAny<string>())).ReturnsAsync(new CommonContracts.Site
            {
                AllocationIds = new List<long> { /* Empty */ }
            });

            var manager = CreateWaterAllocationManager();

            // ACT 
            Func<Task> call = async () => await manager.GetWaterAllocationSiteGeoconnexIntegrationData("test");
            await call.Should().ThrowAsync<WestDaatException>();
        }

        [TestMethod]
        public async Task GetNldiFeatures_Success()
        {
            var faker = new Faker();
            var latitude = faker.Random.Double();
            var longitude = faker.Random.Double();
            var directions = faker.Random.Enum<Common.NldiDirections>();
            var dataPoints = faker.Random.Enum<Common.NldiDataPoints>();

            var resultFeatureCollection = new FeatureCollection();

            _nldiAccessorMock.Setup(a => a.GetNldiFeatures(latitude, longitude, directions, dataPoints))
                             .ReturnsAsync(resultFeatureCollection)
                             .Verifiable();

            var sut = CreateWaterAllocationManager();
            var result = await sut.GetNldiFeatures(latitude, longitude, directions, dataPoints);

            result.Should().Be(resultFeatureCollection);
            _nldiAccessorMock.VerifyAll();
        }

        [TestMethod]
        public async Task FindWaterRights_ResultsReturned()
        {
            //Arrange
            _waterAllocationAccessorMock.Setup(x => x.FindWaterRights(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()))
                .ReturnsAsync(new CommonContracts.WaterRightsSearchResults
                {
                    CurrentPageNumber = 0,
                    WaterRightsDetails = new CommonContracts.WaterRightsSearchDetail[]
                    {
                        new CommonContracts.WaterRightsSearchDetail
                        {
                            AllocationUuid = "abc123"
                        }
                    }
                })
                .Verifiable();

            var searchCriteria = new WaterRightsSearchCriteria();

            //Act
            var manager = CreateWaterAllocationManager();
            var result = await manager.FindWaterRights(searchCriteria);

            //Assert
            result.Should().NotBeNull();
            _waterAllocationAccessorMock.Verify();
        }

        [TestMethod]
        public async Task FindWaterRights_SearchByGeometry_GeoJsonConvertedToGeometry()
        {
            //Arrange
            Geometry[] actualFilterGeometryParam = null;

            _waterAllocationAccessorMock.Setup(x => x.FindWaterRights(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()))
                .Callback((CommonContracts.WaterRightsSearchCriteria x) => actualFilterGeometryParam = x.FilterGeometry)
                .ReturnsAsync(new CommonContracts.WaterRightsSearchResults
                {
                    CurrentPageNumber = 0,
                    WaterRightsDetails = new CommonContracts.WaterRightsSearchDetail[]
                    {
                        new CommonContracts.WaterRightsSearchDetail
                        {
                            AllocationUuid = "abc123"
                        }
                    }
                })
                .Verifiable();

            var searchCriteria = new WaterRightsSearchCriteria
            {
                FilterGeometry = new string[] 
                {
                    "{\"type\":\"Point\",\"coordinates\":[-96.7014,40.8146]}",
                    "{\"coordinates\":[[[-99.88974043488068,42.5970189859552],[-99.89330729367737,42.553083043677304],[-99.78662578967582,42.547588874524024],[-99.78143763142621,42.59487066530488],[-99.88974043488068,42.5970189859552]]],\"type\":\"Polygon\"}"
                }
            };

            var expectedFilterGeometryParam = searchCriteria.FilterGeometry.Select(x => GeometryHelpers.GetGeometryByGeoJson(x));
            expectedFilterGeometryParam.Should().NotBeNullOrEmpty();
            expectedFilterGeometryParam.Should().HaveCount(2);
            expectedFilterGeometryParam.All(x => x != null).Should().BeTrue();

            //Act
            var manager = CreateWaterAllocationManager();
            var result = await manager.FindWaterRights(searchCriteria);

            //Assert
            result.Should().NotBeNull();
            result.WaterRightsDetails.Should().HaveCount(1);
            result.WaterRightsDetails[0].AllocationUuid.Should().Be("abc123");
            actualFilterGeometryParam.Should().NotBeNull();
            actualFilterGeometryParam.Should().BeEquivalentTo(expectedFilterGeometryParam);
        }

        [TestMethod]
        public async Task FindWaterRights_SearchByRiverBasinNames_FeatureConvertedToGeometry()
        {
            //Arrange
            Geometry[] actualFilterGeometryParam = null;

            _waterAllocationAccessorMock.Setup(x => x.FindWaterRights(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()))
                .Callback((CommonContracts.WaterRightsSearchCriteria x) => actualFilterGeometryParam = x.FilterGeometry)
                .ReturnsAsync(new CommonContracts.WaterRightsSearchResults
                {
                    CurrentPageNumber = 0,
                    WaterRightsDetails = new CommonContracts.WaterRightsSearchDetail[]
                    {
                        new CommonContracts.WaterRightsSearchDetail
                        {
                            AllocationUuid = "abc123"
                        }
                    }
                })
                .Verifiable();

            _locationEngineMock.Setup(x => x.GetRiverBasinPolygonsByName(It.Is<string[]>(s => s.Length == 1 && s[0] == ColoradoRiverBasin.BasinName)))
                .Returns(new FeatureCollection(new List<Feature> { ColoradoRiverBasin.Feature }))
                .Verifiable();

            var expectedFilterGeometryParam = GeometryHelpers.GetGeometryByFeatures(new List<Feature> { ColoradoRiverBasin.Feature });

            var searchCriteria = new WaterRightsSearchCriteria
            {
                RiverBasinNames = new[] { ColoradoRiverBasin.BasinName }
            };

            //Act
            var manager = CreateWaterAllocationManager();
            var result = await manager.FindWaterRights(searchCriteria);

            //Assert
            result.Should().NotBeNull();
            result.WaterRightsDetails.Should().HaveCount(1);
            _locationEngineMock.Verify();
            result.WaterRightsDetails[0].AllocationUuid.Should().Be("abc123");
            actualFilterGeometryParam.Should().NotBeNull();
            actualFilterGeometryParam.Should().BeEquivalentTo(expectedFilterGeometryParam);
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetSiteDetails()
        {
            _siteAccessorMock.Setup(x => x.GetSiteDetailsByUuid("TESTME")).ReturnsAsync(new CommonContracts.SiteDetails()).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetSiteDetails("TESTME");

            result.Should().NotBeNull();
            _siteAccessorMock.Verify();
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetWaterSiteLocation()
        {
            var location = new CommonContracts.SiteLocation
            {
                Latitude = 999,
                Longitude = 888,
                PODorPOUSite = "TEST_PODorPOU",
                SiteUuid = "TEST_PODorPOU"
            };

            _siteAccessorMock.Setup(x => x.GetWaterSiteLocationByUuid("TEST_PODorPOU")).ReturnsAsync(location).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetWaterSiteLocation("TEST_PODorPOU");

            result.Should().NotBeNull();
            _siteAccessorMock.Verify();

            result.Properties.First(x => x.Key.ToLower() == "siteuuid").Value.Should().Be(location.SiteUuid);
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetWaterRightDetails()
        {
            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightDetailsById("99")).ReturnsAsync(new CommonContracts.WaterRightDetails()).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetWaterRightDetails("99");

            result.Should().NotBeNull();
            _waterAllocationAccessorMock.Verify();
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetWaterRightSiteInfoList()
        {
            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightSiteInfoById("99")).ReturnsAsync(new List<CommonContracts.SiteInfoListItem>{ }).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetWaterRightSiteInfoList("99");

            result.Should().NotBeNull();
            _waterAllocationAccessorMock.Verify();
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetWaterRightSourceInfoList()
        {
            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightSourceInfoById("99")).ReturnsAsync(new List<CommonContracts.WaterSourceInfoListItem>{ }).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetWaterRightSourceInfoList("99");

            result.Should().NotBeNull();
            _waterAllocationAccessorMock.Verify();
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetWaterRightsDigestsBySite()
        {
            var siteUuid = new Faker().Random.String(11, 'A', 'z');
            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightsDigestsBySite(siteUuid)).ReturnsAsync(new List<CommonContracts.WaterRightsDigest> { }).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetWaterRightsDigestsBySite(siteUuid);

            result.Should().NotBeNull();
            _waterAllocationAccessorMock.Verify();
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetWaterRightSiteLocations()
        {
            var location = new CommonContracts.SiteLocation
            {
                Latitude = 999,
                Longitude = 888,
                PODorPOUSite = "TEST_PODorPOU",
                SiteUuid = "TEST_PODorPOU"
            };

            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightSiteLocationsById("99")).ReturnsAsync(new List<CommonContracts.SiteLocation> { location }).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetWaterRightSiteLocations("99");

            result.Should().NotBeNull();
            _waterAllocationAccessorMock.Verify();

            result.Features.Count.Should().Be(1);
            result.Features[0].Properties.First(x => x.Key.ToLower() == "siteuuid").Value.Should().Be(location.SiteUuid);
        }

        [TestMethod]
        public async Task WaterAllocationManager_GetWaterSiteSourceInfoList()
        {
            _siteAccessorMock.Setup(x => x.GetWaterSiteSourceInfoListByUuid("siteuuid")).ReturnsAsync(new List<CommonContracts.WaterSourceInfoListItem> { }).Verifiable();

            var manager = CreateWaterAllocationManager();
            var result = await manager.GetWaterSiteSourceInfoListByUuid("siteuuid");

            result.Should().NotBeNull();
            _siteAccessorMock.Verify();
        }

        [TestMethod]
        public void ConvertRiverBasinFeaturesToGeometries_Success()
        {
            var basinNames = new List<string>
            {
                ArkansasRiverBasin.BasinName,
                ColoradoRiverBasin.BasinName
            };

            var features = RiverBasinConstants.RiverBasinDictionary.Where(x => basinNames.Contains(x.Key)).Select(x => x.Value).ToList();

            var polygons = GeometryHelpers.GetGeometryByFeatures(features);

            polygons.Should().NotBeNull();
            polygons.Should().HaveCount(2);
        }

        [TestMethod]
        [DataRow(100001)]
        [DataRow(450000)]
        public async Task WaterRightsAsZip_ThrowsException_CountMoreThanPerformanceMaxDownload(int returnAmount)
        {
            var managerSearchRequest = new WaterRightsSearchCriteria
            {
                States = new string[] { "NE" }
            };

            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightsCount(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()))
                .ReturnsAsync(returnAmount)
                .Verifiable();

            var manager = CreateWaterAllocationManager();
            await Assert.ThrowsExceptionAsync<WestDaatException>(() => manager.WaterRightsAsZip(new MemoryStream(), managerSearchRequest));

            _waterAllocationAccessorMock.Verify(x => x.GetWaterRightsCount(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()), Times.Once);
        }

        [TestMethod]
        public async Task WaterRightsAsZip_ThrowsException_WhenAllSearchCriteriaPropertiesAreNull()
        {
            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightsCount(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()))
                .Verifiable();

            var manager = CreateWaterAllocationManager();
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => manager.WaterRightsAsZip(new MemoryStream(), It.IsAny<WaterRightsSearchCriteria>()));

            // throws exception when building the predicate, before this call to happen
            _waterAllocationAccessorMock.Verify(x => x.GetWaterRightsCount(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()), Times.Never);
        }

        [TestMethod]
        public async Task WaterRightsAsZip_BuildsStream()
        {
            // need to get the CSVhelper files
            var variables = new List<CsvModels.Variables>
            {
                new CsvModels.Variables
                {
                    VariableSpecificUuid = Guid.NewGuid().ToString()
                },
                new CsvModels.Variables
                {
                    VariableSpecificUuid = Guid.NewGuid().ToString()
                },
                new CsvModels.Variables
                {
                    VariableSpecificUuid = Guid.NewGuid().ToString()
                }
            };

            var organizations = new List<CsvModels.Organizations>
            {
                new CsvModels.Organizations
                {
                    OrganizationUuid = Guid.NewGuid().ToString()
                },
                new CsvModels.Organizations
                {
                    OrganizationUuid = Guid.NewGuid().ToString()
                },
                new CsvModels.Organizations
                {
                    OrganizationUuid = Guid.NewGuid().ToString()
                }
            };

            var iEnumerableList = new List<IEnumerable<dynamic>>
            {
                variables,
                organizations
            };

            _waterAllocationAccessorMock.Setup(x => x.GetWaterRightsCount(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()))
                .ReturnsAsync(5)
                .Verifiable();

            _waterAllocationAccessorMock.Setup(x => x.GetWaterRights(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()))
                .Returns(iEnumerableList)
                .Verifiable();

            _templateResourceSdk.Setup(s => s.GetTemplate(Common.ResourceType.Citation))
                .Returns(_citationFile);

            var managerSearchRequest = new WaterRightsSearchCriteria
            {
                States = new string[] { "NE" }
            };

            var memoryStream = new MemoryStream();

            var manager = CreateWaterAllocationManager();
            await manager.WaterRightsAsZip(memoryStream, managerSearchRequest);

            var reader = new StreamReader(memoryStream);

            _waterAllocationAccessorMock.Verify(x => x.GetWaterRightsCount(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()), Times.Once);
            _waterAllocationAccessorMock.Verify(x => x.GetWaterRights(It.IsAny<CommonContracts.WaterRightsSearchCriteria>()), Times.Once);
        }

        private IWaterAllocationManager CreateWaterAllocationManager()
        {
            return new WaterAllocationManager(
                _nldiAccessorMock.Object,
                _siteAccessorMock.Object,
                _waterAllocationAccessorMock.Object,
                _geoConnexEngineMock.Object,
                _locationEngineMock.Object,
                _templateResourceSdk.Object,
                _performanceConfiguration.Object,
                CreateLogger<WaterAllocationManager>()
            );
        }
    }
}
