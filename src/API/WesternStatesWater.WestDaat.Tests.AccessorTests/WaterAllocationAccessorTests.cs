﻿using WesternStatesWater.WestDaat.Accessors;
using WesternStatesWater.WestDaat.Tests.Helpers;
using EF = WesternStatesWater.WestDaat.Accessors.EntityFramework;
using CommonContracts = WesternStatesWater.WestDaat.Common.DataContracts;

namespace WesternStatesWater.WestDaat.Tests.AccessorTests
{
    [TestClass]
    [TestCategory("Accessor Tests")]
    public class WaterAllocationAccessorTests : AccessorTestBase
    {
        [TestMethod]
        [DataRow("expectedName", "name2", "expectedName")]
        [DataRow("", "expectedName", "expectedName")]
        [DataRow("expectedName", "", "expectedName")]
        public async Task FindWaterRights_SearchByBeneficalUse_ReturnsOneMatch(string beneficialUseWadeName, string beneficialUseName, string expectedName)
        {
            // Arrange
            var beneficialUses = new BeneficialUsesCVFaker().Generate(1);
            beneficialUses[0].WaDEName = beneficialUseWadeName;
            beneficialUses[0].Name = beneficialUseName;

            var allocationAmountTarget = new AllocationAmountFactFaker()
                .LinkBeneficialUses(beneficialUses.ToArray())
                .Generate();
            var allocationAmountOthers = new AllocationAmountFactFaker()
                .LinkBeneficialUses(new BeneficialUsesCVFaker().Generate(1).ToArray())
                .Generate(3);
            using (var db = CreateDatabaseContextFactory().Create())
            {
                db.AllocationAmountsFact.Add(allocationAmountTarget);
                db.AllocationAmountsFact.AddRange(allocationAmountOthers);
                db.SaveChanges();
            }

            var searchCriteria = new CommonContracts.WaterRightsSearchCriteria
            {
                BeneficialUses = new[] { expectedName }
            };

            //Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.FindWaterRights(searchCriteria);

            //Assert
            result.WaterRightsDetails.Count().Should().Be(1);
            result.WaterRightsDetails.FirstOrDefault().BeneficialUses.Should().Contain(expectedName);
        }

        [TestMethod]
        [DataRow("expectedName", "name2", "expectedName")]
        [DataRow("", "expectedName", "expectedName")]
        public async Task FindWaterRights_OwnerClassification_MatchByWadeNameOrName(string ownerClassificationWadeName, string ownerClassificationName, string searchInput)
        {
            // Arrange
            var allocationAmounts = new AllocationAmountFactFaker()
                .IncludeOwnerClassification()
                .Generate(4);

            allocationAmounts[2].OwnerClassificationCV = ownerClassificationName;
            allocationAmounts[2].OwnerClassification.WaDEName = ownerClassificationWadeName;
            allocationAmounts[2].OwnerClassification.Name = ownerClassificationName;            

            using (var db = CreateDatabaseContextFactory().Create())
            {
                db.AllocationAmountsFact.AddRange(allocationAmounts);
                db.SaveChanges();
            }

            var expectedWadeUuid = allocationAmounts[2].AllocationAmountId.ToString();

            var searchCriteria = new CommonContracts.WaterRightsSearchCriteria
            {
                OwnerClassifications = new[] { searchInput }
            };

            //Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.FindWaterRights(searchCriteria);

            //Assert
            result.WaterRightsDetails.Count().Should().Be(1);
            result.WaterRightsDetails.FirstOrDefault().WadeUuid.Should().Be(expectedWadeUuid);
            result.WaterRightsDetails.FirstOrDefault().OwnerClassification.Should().Be(searchInput);
        }

        [TestMethod]
        [DataRow(10, 0, new string[] { "expectedName0", "expectedName1" })]
        [DataRow(4, 1, new string[] { "expectedName0", "expectedName1" })]
        [DataRow(10, 2, new string[] { "expectedName0", "expectedName1", "expectedName2" })]
        [DataRow(10, 4, new string[] { "expectedName0" })]
        public async Task FindWaterRights_SearchByOwnerClassification_ReturnsMatches(int totalRecordCount, int expectedResultCount, string[] searchInputs)
        {
            // Arrange
            var rand = new Random();

            var expectedResults = new List<(string wadeUuid, string ownerClassification)>();

            var ownerClassificationCVs = new List<EF.OwnerClassificationCv>();

            // generate ownerClassificationsCV objects that match search inputs
            foreach (var input in searchInputs)
            {
                var ownerClassificationFake = new OwnerClassificationCvFaker().Generate();
                ownerClassificationFake.WaDEName = input;
                ownerClassificationFake.Name = input;

                ownerClassificationCVs.Add(ownerClassificationFake);
            }

            var allocationAmounts = new AllocationAmountFactFaker().Generate(totalRecordCount);

            // assign the the ownerClassifications
            for (int i = 0; i < expectedResultCount; i++)
            {
                var input = searchInputs[rand.Next(0, searchInputs.Length)];

                allocationAmounts[i].OwnerClassificationCV = input;
                allocationAmounts[i].OwnerClassification = ownerClassificationCVs.First(x => x.WaDEName == input);                
            }

            // create additional random ownerClassifications
            for (int i = expectedResultCount; i < totalRecordCount; i++)
            {
                var ownerClassificationFake = new OwnerClassificationCvFaker().Generate();
                allocationAmounts[i].OwnerClassificationCV = ownerClassificationFake.Name;
                allocationAmounts[i].OwnerClassification = ownerClassificationFake;
            }

            using (var db = CreateDatabaseContextFactory().Create())
            {
                db.AllocationAmountsFact.AddRange(allocationAmounts);
                db.SaveChanges();
            }

            expectedResults.AddRange(allocationAmounts
                .Take(expectedResultCount)
                .Select(x => (wadeUuid: x.AllocationAmountId.ToString(), ownerClassification: x.OwnerClassification.WaDEName)));

            var searchCriteria = new CommonContracts.WaterRightsSearchCriteria
            {
                OwnerClassifications = searchInputs
            };

            //Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.FindWaterRights(searchCriteria);

            //Assert
            expectedResults.Count.Should().Be(expectedResultCount);

            result.WaterRightsDetails.Count().Should().Be(expectedResultCount);

            expectedResults.TrueForAll(expected => result.WaterRightsDetails.SingleOrDefault(actual => actual.WadeUuid == expected.wadeUuid) != null);

            foreach (var expectedResult in expectedResults)
            {
                var waterRight = result.WaterRightsDetails.FirstOrDefault(actual => actual.WadeUuid == expectedResult.wadeUuid);
                waterRight.Should().NotBeNull();
                searchInputs.Should().Contain(waterRight.OwnerClassification);
            }
        }

        [TestMethod]
        [DataRow("expectedName", "name2", "expectedName")]
        [DataRow("", "expectedName", "expectedName")]
        public async Task FindWaterRights_WaterSourceTypes_MatchByWadeNameOrName(string waterSourceTypeWadeName, string waterSourceTypeName, string searchInput)
        {
            // Arrange
            var waterSourceType = new WaterSourceTypeFaker().Generate();
            waterSourceType.WaDEName = waterSourceTypeWadeName;
            waterSourceType.Name = waterSourceTypeName;

            var waterSourceFake = new WaterSourceDimFaker().Generate();
            waterSourceFake.WaterSourceTypeCv = waterSourceTypeName;
            waterSourceFake.WaterSourceTypeCvNavigation = waterSourceType;

            var siteFake = new SitesDimFaker()
                    .LinkWaterSources(waterSourceFake)
                    .Generate();

            var allocationAmounts = new AllocationAmountFactFaker().Generate(3);
            allocationAmounts.Add(new AllocationAmountFactFaker()
                    .LinkSites(siteFake)
                    .Generate());            

            using (var db = CreateDatabaseContextFactory().Create())
            {
                db.AllocationAmountsFact.AddRange(allocationAmounts);
                db.SaveChanges();
            }

            var expectedWadeUuid = allocationAmounts[3].AllocationAmountId.ToString();

            var searchCriteria = new CommonContracts.WaterRightsSearchCriteria
            {
                WaterSourceTypes = new[] { searchInput }
            };

            //Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.FindWaterRights(searchCriteria);

            //Assert
            result.WaterRightsDetails.Count().Should().Be(1);
            result.WaterRightsDetails.FirstOrDefault().WadeUuid.Should().Be(expectedWadeUuid);
        }

        [TestMethod]
        [DataRow(10, 0, new string[] { "expectedName0", "expectedName1" })]
        [DataRow(4, 1, new string[] { "expectedName0", "expectedName1" })]
        [DataRow(10, 2, new string[] { "expectedName0", "expectedName1", "expectedName2" })]
        [DataRow(10, 4, new string[] { "expectedName0" })]
        public async Task FindWaterRights_SearchByWaterSourceTypes_ReturnsMatches(int totalRecordCount, int expectedResultCount, string[] searchInputs)
        {
            // Arrange
            var rand = new Random();

            var expectedResults = new List<string>();

            var waterSourceTypes = new List<EF.WaterSourceType>();

            // generate waterSourceType objects that match search inputs
            foreach (var input in searchInputs)
            {
                var waterSourceType = new WaterSourceTypeFaker().Generate();
                waterSourceType.WaDEName = input;
                waterSourceType.Name = input;

                waterSourceTypes.Add(waterSourceType);
            }

            var matchingAllocationAmounts = new List<EF.AllocationAmountsFact>();

            // generate waterSource objects that match search inputs
            for (int i = 0; i < expectedResultCount; i++)
            {
                var input = searchInputs[rand.Next(0, searchInputs.Length)];
                var waterSourceFake = new WaterSourceDimFaker().Generate();
                waterSourceFake.WaterSourceTypeCv = input;
                waterSourceFake.WaterSourceTypeCvNavigation = waterSourceTypes.First(x => x.WaDEName == input);

                var siteFake = new SitesDimFaker()
                    .LinkWaterSources(waterSourceFake)
                    .Generate();

                matchingAllocationAmounts.Add(new AllocationAmountFactFaker()
                    .LinkSites(siteFake)
                    .Generate());
            }            

            // generate waterSource objects that don't match
            var notMatchingAllocationAmounts = new AllocationAmountFactFaker()
                .LinkSites(new SitesDimFaker()
                    .LinkWaterSources(new WaterSourceDimFaker().Generate(3).ToArray())
                    .Generate(1).ToArray()
                    )
                .Generate(totalRecordCount - expectedResultCount);

            using (var db = CreateDatabaseContextFactory().Create())
            {
                db.AllocationAmountsFact.AddRange(matchingAllocationAmounts);
                db.AllocationAmountsFact.AddRange(notMatchingAllocationAmounts);
                db.SaveChanges();
            }

            expectedResults.AddRange(matchingAllocationAmounts.Select(x => x.AllocationAmountId.ToString()));

            var searchCriteria = new CommonContracts.WaterRightsSearchCriteria
            {
                WaterSourceTypes = searchInputs
            };

            //Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.FindWaterRights(searchCriteria);

            //Assert
            expectedResults.Count.Should().Be(expectedResultCount);

            result.WaterRightsDetails.Count().Should().Be(expectedResultCount);

            expectedResults.TrueForAll(expected => result.WaterRightsDetails.SingleOrDefault(actual => actual.WadeUuid == expected) != null);

            foreach (var expectedResult in expectedResults)
            {
                var waterRight = result.WaterRightsDetails.FirstOrDefault(actual => actual.WadeUuid == expectedResult);
                waterRight.Should().NotBeNull();
            }
        }

        [TestMethod]
        [DataRow(10, 0, new string[] { "ZZ" })]
        [DataRow(5, 5, new string[] { "XX", "ZZ" })]
        [DataRow(10, 2, new string[] { "YY", "NN", "ZZ"})]
        public async Task FindWaterRights_SearchByStates_ReturnsMatches(int totalRecordCount, int expectedResultCount, string[] searchInputs)
        {
            //Arrange
            var rand = new Random();

            var matchedOrganizations = new List<EF.OrganizationsDim>();

            // generate an orgainization for each search param
            foreach(var input in searchInputs)
            {
                var organizationFake = new OrganizationsDimFaker().Generate();
                organizationFake.State = input;

                matchedOrganizations.Add(organizationFake);
            }
            
            var matchedAllocationAmounts = new List<EF.AllocationAmountsFact>();

            var expectedWadeUuids = new List<string>();

            // generate matching allocationAmouts
            for (int i = 0; i < expectedResultCount; i++)
            {
                var organization = matchedOrganizations[rand.Next(0, matchedOrganizations.Count)];
                var allocationAmount = new AllocationAmountFactFaker()
                    .LinkOrganizaion(organization).Generate();

                expectedWadeUuids.Add(allocationAmount.AllocationAmountId.ToString());
                matchedAllocationAmounts.Add(allocationAmount);
            }
            
            //generate non-matching organizations
            var nonMatchedOrgizations = new List<EF.OrganizationsDim>(new OrganizationsDimFaker().Generate(5));

            //generate non-matching allocationAmounts
            var nonMatchedAllocationAmounts = new List<EF.AllocationAmountsFact>();
            for (int i = 0; i < totalRecordCount - expectedResultCount; i++)
            {
                var organization = nonMatchedOrgizations[rand.Next(0, nonMatchedOrgizations.Count)];
                nonMatchedAllocationAmounts.Add(new AllocationAmountFactFaker()
                    .LinkOrganizaion(organization).Generate());
            }

            using (var db = CreateDatabaseContextFactory().Create())
            {
                db.AllocationAmountsFact.AddRange(matchedAllocationAmounts);
                db.AllocationAmountsFact.AddRange(nonMatchedAllocationAmounts);
                db.SaveChanges();
            }

            var searchCriteria = new CommonContracts.WaterRightsSearchCriteria
            {
                States = searchInputs
            };

            //Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.FindWaterRights(searchCriteria);

            result.WaterRightsDetails.Should().NotBeNull();
            result.WaterRightsDetails.Should().HaveCount(expectedResultCount);

            expectedWadeUuids.TrueForAll(expected => result.WaterRightsDetails.SingleOrDefault(actual => actual.WadeUuid == expected) != null);
        }

        [TestMethod]
        public void WaterAllocationAccessor_GetWaterAllocationAmountOrganizationById_ShouldReturnOrg()
        {
            // Arrange
            var allocationAmount = new AllocationAmountFactFaker().Generate();
            using (var db = CreateDatabaseContextFactory().Create())
            {
                db.AllocationAmountsFact.Add(allocationAmount);
                db.SaveChanges();
            }

            var expectedOrg = allocationAmount.Organization;
            var allocationAmountId = allocationAmount.AllocationAmountId;

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = accessor.GetWaterAllocationAmountOrganizationById(allocationAmountId);

            // Assert
            result.Should().NotBeNull();
            result.OrganizationDataMappingUrl.Should().NotBeNullOrWhiteSpace();
            result.OrganizationDataMappingUrl.Should().Be(expectedOrg.OrganizationDataMappingUrl);
            result.OrganizationName.Should().Be(expectedOrg.OrganizationName);
            result.OrganizationId.Should().Be(expectedOrg.OrganizationId);
        }

        [TestMethod]
        public async Task GetWaterRightDetailsById_Matches()
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();

            var allocationAmount = new AllocationAmountFactFaker().Generate();
            allocationAmount.VariableSpecific.AggregationInterval = 5.0M;
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            var expectedOrg = allocationAmount.Organization;
            var expectedVariable = allocationAmount.VariableSpecific;
            var allocationAmountId = allocationAmount.AllocationAmountId;

            var expectedResult = new CommonContracts.WaterRightDetails
            {
                MethodDescription = allocationAmount.Method.MethodDescription,
                MethodType = allocationAmount.Method.MethodTypeCv,
                AggregationInterval = expectedVariable.AggregationInterval,
                AggregationIntervalUnit = expectedVariable.AggregationIntervalUnitCv,
                AggregationStatistic = expectedVariable.AggregationStatisticCv,
                AmountUnitCv = expectedVariable.AmountUnitCv,
                ApplicableResourceType = allocationAmount.Method.ApplicableResourceTypeCv,
                MethodLink = allocationAmount.Method.MethodNemilink,
                OrganizationName = expectedOrg.OrganizationName,
                OrganizationWebsite = expectedOrg.OrganizationWebsite,
                ReportYearStartMonth = expectedVariable.ReportYearStartMonth,
                ReportYearTypeCv = expectedVariable.ReportYearTypeCv,
                State = expectedOrg.State,
                VariableCv = expectedVariable.VariableCv,
                VariableSpecific = expectedVariable.VariableSpecificCv,
            };

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightDetailsById(allocationAmountId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task GetWaterRightDetailsById_NoMatch()
        {
            // Act
            var accessor = CreateWaterAllocationAccessor();
            Func<Task> call = async () => await accessor.GetWaterRightDetailsById(1234);

            // Assert
            await call.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task WaterAllocationAccessor_GetWaterRightSiteInfoById()
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();
            var sites = new SitesDimFaker().Generate(5);
            db.SitesDim.AddRange(sites);
            db.SaveChanges();

            var allocationAmount = new AllocationAmountFactFaker().Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            foreach (var site in sites)
            {
                var allocationSiteBridge = new AllocationBridgeSiteFactFaker()
                    .AllocationBridgeSiteFactFakerWithIds(allocationAmount.AllocationAmountId, site.SiteId)
                    .Generate();
                db.Add(allocationSiteBridge);
            }
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightSiteInfoById(allocationAmount.AllocationAmountId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            result.Any(x => x.SiteUuid == sites[0].SiteUuid).Should().BeTrue();
        }

        [TestMethod]
        public async Task WaterAllocationAccessor_GetWaterRightSourceInfoById()
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();
            var sites = new SitesDimFaker().Generate(5);
            db.SitesDim.AddRange(sites);
            db.SaveChanges();

            var allocationAmount = new AllocationAmountFactFaker().Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            var waterSources = new WaterSourceDimFaker().Generate(5);
            db.WaterSourcesDim.AddRange(waterSources);
            db.SaveChanges();


            foreach (var site in sites)
            {
                var allocationSiteBridge = new AllocationBridgeSiteFactFaker()
                    .AllocationBridgeSiteFactFakerWithIds(allocationAmount.AllocationAmountId, site.SiteId)
                    .Generate();
                db.AllocationBridgeSitesFact.Add(allocationSiteBridge);

                foreach (var waterSource in waterSources)
                {
                    var waterSourceBridge = new WaterSourceBridgeSiteFactFaker()
                        .WaterSourceBridgeSiteFactFakerWithIds(waterSource.WaterSourceId, site.SiteId)
                        .Generate();
                    db.WaterSourceBridgeSitesFact.Add(waterSourceBridge);
                }
            }
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightSourceInfoById(allocationAmount.AllocationAmountId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            result.Any(x => x.WaterSourceUuid == waterSources[0].WaterSourceUuid).Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("2022-01-22")]
        public async Task GetAllWaterAllocations_PriorityDate(string dateValue)
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();

            DateTime? date = dateValue == null ? null : DateTime.Parse(dateValue);

            var allocationAmount = new AllocationAmountFactFaker()
                .SetAllocationPriorityDate(date)
                .Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetAllWaterAllocations();

            // Assert
            result.Should().NotBeNull().And
                .HaveCount(1).And
                .Contain(a => a.AllocationPriorityDate == date);
        }

        [TestMethod]
        public async Task GetWaterRightsDigestsBySite_Matches()
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();

            var date = new Faker().Date.Past(150);
            var site = new SitesDimFaker().Generate();

            var allocationAmount = new AllocationAmountFactFaker()
                .SetAllocationPriorityDate(date)
                .LinkSites(site)
                .Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightsDigestsBySite(site.SiteUuid);

            // Assert
            result.Should().NotBeNull().And
                .BeEquivalentTo(new[]
                {
                    new CommonContracts.WaterRightsDigest
                    {
                        Id = allocationAmount.AllocationAmountId,
                        NativeId = allocationAmount.AllocationNativeId,
                        PriorityDate = date.Date,
                        BeneficialUses = new List<string>()
                    }
                });
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetWaterRightsDigestsBySite_Multiples(int allocationCount)
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();

            var site = new SitesDimFaker().Generate();
            db.SitesDim.Add(site);

            var allocationAmount = new AllocationAmountFactFaker()
                .LinkSites(site)
                .Generate(allocationCount);
            db.AllocationAmountsFact.AddRange(allocationAmount);
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightsDigestsBySite(site.SiteUuid);

            // Assert
            var dbSites = db.SitesDim.ToList();
            dbSites.Should().HaveCount(1).And
                .AllSatisfy(a => a.SiteUuid.Should().Be(site.SiteUuid));
            result.Select(a => a.Id).Should()
                .BeEquivalentTo(allocationAmount.Select(a => a.AllocationAmountId));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public async Task GetWaterRightsDigestsBySite_BeneficialUses(int beneficialUseCount)
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();

            var site = new SitesDimFaker().Generate();
            var beneficialUses = new BeneficialUsesCVFaker().Generate(beneficialUseCount);

            var allocationAmount = new AllocationAmountFactFaker()
                .LinkSites(site)
                .LinkBeneficialUses(beneficialUses.ToArray())
                .Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightsDigestsBySite(site.SiteUuid);

            // Assert
            result
                .Should().NotBeNull().And
                .HaveCount(1);

            result[0].BeneficialUses
                .Should().NotBeNull().And
                .BeEquivalentTo(beneficialUses.Select(a => a.Name));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("2022-01-22")]
        public async Task GetWaterRightsDigestsBySite_Dates(string dateValue)
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();

            var site = new SitesDimFaker().Generate();

            DateTime? date = dateValue == null ? null : DateTime.Parse(dateValue);

            var allocationAmount = new AllocationAmountFactFaker()
                .LinkSites(site)
                .SetAllocationPriorityDate(date)
                .Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightsDigestsBySite(site.SiteUuid);

            // Assert
            result
                .Should().NotBeNull().And
                .HaveCount(1);

            result[0].PriorityDate.Should().Be(date);
        }

        [TestMethod]
        [TestCategory("Accessor Tests")]
        public async Task WaterAllocationAccessor_GetWaterRightSiteLocationsById()
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();
            var sites = new SitesDimFaker().Generate(5);
            db.SitesDim.AddRange(sites);
            db.SaveChanges();

            var allocationAmount = new AllocationAmountFactFaker().Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            foreach (var site in sites)
            {
                var allocationSiteBridge = new AllocationBridgeSiteFactFaker()
                    .AllocationBridgeSiteFactFakerWithIds(allocationAmount.AllocationAmountId, site.SiteId)
                    .Generate();
                db.Add(allocationSiteBridge);
            }
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightSiteLocationsById(allocationAmount.AllocationAmountId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            result.Any(x => x.Latitude == sites[0].Latitude).Should().BeTrue();
        }

        [TestMethod]
        [TestCategory("Accessor Tests")]
        public async Task WaterAllocationAccessor_GetWaterRightSiteLocationsById_SkipNulls()
        {
            // Arrange
            using var db = CreateDatabaseContextFactory().Create();
            var sites = new SitesDimFaker().Generate(5);
            db.SitesDim.AddRange(sites);
            db.SaveChanges();

            var allocationAmount = new AllocationAmountFactFaker().Generate();
            db.AllocationAmountsFact.Add(allocationAmount);
            db.SaveChanges();

            var nullLocationSite = new SitesDimFaker().Generate();
            nullLocationSite.Latitude = null;
            nullLocationSite.Longitude = null;
            db.SitesDim.Add(nullLocationSite);
            db.SaveChanges();

            sites.Add(nullLocationSite);

            foreach (var site in sites)
            {
                var allocationSiteBridge = new AllocationBridgeSiteFactFaker()
                    .AllocationBridgeSiteFactFakerWithIds(allocationAmount.AllocationAmountId, site.SiteId)
                    .Generate();
                db.Add(allocationSiteBridge);
            }
            db.SaveChanges();

            // Act
            var accessor = CreateWaterAllocationAccessor();
            var result = await accessor.GetWaterRightSiteLocationsById(allocationAmount.AllocationAmountId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(5);
        }

        private IWaterAllocationAccessor CreateWaterAllocationAccessor()
        {
            return new WaterAllocationAccessor(CreateLogger<WaterAllocationAccessor>(), CreateDatabaseContextFactory());
        }
    }
}
