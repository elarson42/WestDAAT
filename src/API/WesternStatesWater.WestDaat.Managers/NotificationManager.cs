﻿using Microsoft.Extensions.Logging;
using System.Net.Mail;
using WesternStatesWater.WestDaat.Contracts.Client;
using WesternStatesWater.WestDaat.Utilities;
using CommonDTO = WesternStatesWater.WestDaat.Common.DataContracts;

namespace WesternStatesWater.WestDaat.Managers
{
    public sealed class NotificationManager : ManagerBase, INotificationManager
    {
        private readonly IEmailNotificationSDK _emailSDK;

        public NotificationManager(IEmailNotificationSDK emailSDK, ILogger<NotificationManager> logger) : base(logger)
        {
            _emailSDK = emailSDK;
        }

        async Task<bool> INotificationManager.PostFeedback(FeedbackRequest request)
        {
            var emailAddresses = new List<string> { "adelabdallah@wswc.utah.gov", "rjames@wswc.utah.gov" };
            if (IsValidEmail(request.Email))
            {
                emailAddresses.Add(request.Email);
            }
            var messageBody = FeedbackMessageBody(request);

            var msg = new CommonDTO.EmailRequest()
            {
                Subject = "WestDAAT Feedback",
                TextContent = "Please Enable HTML to display this message",
                Body = messageBody,
                From = "adelabdallah@wswc.utah.gov",
                To = emailAddresses.ToArray()
            };

            var response = await _emailSDK.SendEmail(msg);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError($"Something went wrong while sending feedback email.\nStatusCode: {response.StatusCode}.\nMessageBody: {messageBody}.");
            }

            return response.IsSuccessStatusCode;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string FeedbackMessageBody(FeedbackRequest request)
        {
            var messageBody = "<div style=\"padding-left: 30px;\">";

            if (!string.IsNullOrWhiteSpace(request.SatisfactionLevel))
            {
                messageBody += GetMessageSection(nameof(request.SatisfactionLevel), request.SatisfactionLevel);
            }

            if (!string.IsNullOrWhiteSpace(request.Comments))
            {
                messageBody += GetMessageSection(nameof(request.Comments), request.Comments);
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                messageBody += GetMessageSection(nameof(request.Email), request.Email);
            }

            if (!string.IsNullOrWhiteSpace(request.Organization))
            {
                messageBody += GetMessageSection(nameof(request.Organization), request.Organization);
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                messageBody += GetMessageSection(nameof(request.Role), request.Role);
            }

            if (request.DataUsage.Any())
            {
                messageBody += "<div style=\"padding-top: 20px;\">" +
                    "<label style=\"font-size: 20px; font-weight:bold; color: #003d4c; display:block; border-bottom: 1px solid #ccc;\">Data Usage</label>" +
                    "<ul style=\"margin-left: 10px; padding-left: 10px; margin-bottom: 0px;\">";
                foreach(var use in request.DataUsage)
                {
                    messageBody += $"<li><label>{use}</label></li>";
                }
                messageBody += "</div>";
            }

            if (request.DataInterest.Any())
            {
                messageBody += "<div style=\"padding-top: 20px;\">" +
                    "<label style=\"font-size: 20px; font-weight:bold; color: #003d4c; display:block; border-bottom: 1px solid #ccc;\">Data Interest</label>" +
                    "<ul style=\"margin-left: 10px; padding-left: 10px; margin-bottom: 0px;\">";
                foreach (var interest in request.DataInterest)
                {
                    messageBody += $"<li><label>{interest}</label></li>";
                }
                messageBody += "</div>";
            }

            return messageBody += "</div>";
        }

        private string GetMessageSection(string propertyName, string value)
        {
            return $"<div style=\"padding-top:20px;\">" +
                $"<label style=\"font-size: 20px; font-weight:bold; color: #003d4c; display:block; border-bottom: 1px solid #ccc;\">{propertyName}</label>" +
                $"<label style=\"padding-left: 20px;\">{value}</label>" +
                $"</div>";
        }
    }
}
