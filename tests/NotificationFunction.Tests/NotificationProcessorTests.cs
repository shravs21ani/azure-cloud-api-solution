using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NotificationFunction.Functions;

namespace NotificationFunction.Tests
{
    public class NotificationProcessorTests
    {
        private NotificationProcessor _notificationProcessor;
        private Mock<ILogger<NotificationProcessor>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<NotificationProcessor>>();
            _notificationProcessor = new NotificationProcessor(_loggerMock.Object);
        }

        [Test]
        public async Task Run_ValidMessage_ProcessesSuccessfully()
        {
            // Arrange
            var message = new ServiceBusReceivedMessage
            {
                Body = new BinaryData("{\"Id\":\"1\",\"Message\":\"Test Notification\",\"Timestamp\":\"2023-01-01T00:00:00Z\"}"),
                MessageId = "1"
            };

            // Act
            await _notificationProcessor.Run(message, null);

            // Assert
            _loggerMock.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Run_InvalidMessage_LogsError()
        {
            // Arrange
            var message = new ServiceBusReceivedMessage
            {
                Body = new BinaryData("Invalid JSON"),
                MessageId = "2"
            };

            // Act
            await _notificationProcessor.Run(message, null);

            // Assert
            _loggerMock.Verify(logger => logger.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}