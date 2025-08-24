using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using NotificationFunction.Models;

namespace NotificationFunction.Functions
{
    public class NotificationProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NotificationProcessor> _logger;

        public NotificationProcessor(HttpClient httpClient, ILogger<NotificationProcessor> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [Function("NotificationProcessor")]
        public async Task Run(
            [ServiceBusTrigger("notification-queue", Connection = "ServiceBusConnection")] string queueItem)
        {
            try
            {
                var notification = JsonConvert.DeserializeObject<Notification>(queueItem);
                _logger.LogInformation($"Processing notification: {notification.Message}");

                // Process the notification (e.g., send an email, log to a database, etc.)
                await ProcessNotification(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing notification: {ex.Message}");
                throw; // Optionally rethrow to trigger DLQ
            }
        }

        private async Task ProcessNotification(Notification notification)
        {
            // Example of sending a notification (e.g., HTTP request to another service)
            var content = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://example.com/api/notifications", content);
            response.EnsureSuccessStatusCode();
        }
    }
}