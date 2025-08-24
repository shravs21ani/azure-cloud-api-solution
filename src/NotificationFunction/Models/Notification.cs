using System;

namespace NotificationFunction.Models
{
    public class Notification
    {
        public required string Id { get; set; }
        public required string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}