using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationFunction.Models;
using System.Net.Http;

namespace NotificationFunction.Functions;

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
        [ServiceBusTrigger("notification-queue", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage queueItem)
    {
        try
        {
            if (queueItem is null)
            {
                _logger.LogWarning("Null message");
                return;
            }

            // Safely read the message body
            var body = Encoding.UTF8.GetString(queueItem.Body.ToArray());
            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogWarning("Empty body");
                return;
            }

            // Newtonsoft returns nullable; handle invalid JSON
            Notification? notification;
            try
            {
                notification = JsonConvert.DeserializeObject<Notification>(body);
            }
            catch (JsonException jex)
            {
                _logger.LogError(jex, "Invalid JSON payload: {Body}", body);
                return;
            }

            if (notification is null)
            {
                _logger.LogWarning("Deserialized notification is null. Body: {Body}", body);
                return;
            }

            _logger.LogInformation("Processing notification: {Message}", notification.Message);
            await ProcessNotification(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification");
            throw; // let Functions retry / DLQ
        }
    }

    private async Task ProcessNotification(Notification notification)
    {
        var content = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://example.com/api/notifications", content);
        response.EnsureSuccessStatusCode();
    }
}