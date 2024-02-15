
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Xunit;

namespace TestTraineeCamp
{
    public class BlobTriggerCSharpTests
    {
        [Fact]
        public void BlobTriggerCSharp_Run_ShouldLogMessage()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var stream = new MemoryStream();

            // Act
            BlobTriggerCSharp.Run(stream, "test.txt", mockLogger.Object);

            // Assert
            mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(v => v.ToString().Contains("C# Blob trigger function Processed blob")),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()));
        }
    }
}