﻿using CommonDTO = WesternStatesWater.WestDaat.Common.DataContracts;
using WesternStatesWater.WestDaat.Utilities;
using WesternStatesWater.WestDaat.Contracts.Client;
using WesternStatesWater.WestDaat.Managers;

namespace WesternStatesWater.WestDaat.Tests.ManagerTests
{
    [TestClass]
    public class NotificationManagerTests : ManagerTestBase
    {
        private readonly Mock<IEmailNotificationSDK> _emailNotificationSDK = new(MockBehavior.Strict);
        private INotificationManager Manager { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Manager = CreateNotificationManager();
        }

        [TestMethod]
        public void SendFeedback_Success()
        {
            var faker = new Faker();

            var feedbackRequest = new FeedbackRequest
            {
                Name = faker.Random.String(),
                LastName = faker.Random.String(),
                Email = faker.Random.String(),
                Organization = faker.Random.String(),
                Role = faker.Random.String(),
                Comments = faker.Random.String(),
                SatisfactionLevel = faker.Random.String(),
                DataInterest = new string[] { faker.Random.String(), faker.Random.String() },
                DataUsage = new string[] { faker.Random.String(), faker.Random.String() }
            };

            CommonDTO.EmailRequest emailRequest = null;

            _emailNotificationSDK.Setup(a => a.SendFeedback(It.IsAny<CommonDTO.EmailRequest>()))
                             .Callback<CommonDTO.EmailRequest>((request) => { emailRequest = request; });

            Manager.SendFeedback(feedbackRequest);

            emailRequest.Should().NotBeNull();
            emailRequest.Subject.Should().NotBeNull();
            emailRequest.Subject.Should().Be("WestDAAT Feedback");
            emailRequest.To.Should().BeNullOrEmpty();
            emailRequest.From.Should().BeNull();
            emailRequest.TextContent.Should().NotBeNullOrEmpty();
            emailRequest.TextContent.Should().Contain(feedbackRequest.Name);
            emailRequest.TextContent.Should().Contain(feedbackRequest.LastName);
            emailRequest.TextContent.Should().Contain(feedbackRequest.SatisfactionLevel);
            emailRequest.TextContent.Should().Contain(feedbackRequest.Organization);
            emailRequest.TextContent.Should().Contain(feedbackRequest.Role);
            emailRequest.TextContent.Should().Contain(feedbackRequest.Comments);
            emailRequest.TextContent.Should().Contain(feedbackRequest.Email);
            emailRequest.TextContent.Should().Contain(feedbackRequest.DataInterest[0]);
            emailRequest.TextContent.Should().Contain(feedbackRequest.DataInterest[1]);
            emailRequest.TextContent.Should().Contain(feedbackRequest.DataUsage[0]);
            emailRequest.TextContent.Should().Contain(feedbackRequest.DataUsage[1]);
            emailRequest.Body.Should().BeNullOrWhiteSpace();
        }

        private INotificationManager CreateNotificationManager()
        {
            return new NotificationManager(
                _emailNotificationSDK.Object,
                CreateLogger<NotificationManager>()
            );
        }
    }
}
