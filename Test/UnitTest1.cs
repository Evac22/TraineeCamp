using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;

namespace BlobTriggerFunction.Tests
{
    public class BlobTriggerFunctionTest
    {
        [Fact]
        public void Run_ShouldLogMessage_WhenBlobIsProcessed()
        {
            // Arrange
            var mockBlob = new Mock<CloudBlockBlob>(new Uri("https://fake.blob.core.windows.net/container/blob"));
            mockBlob.Setup(b => b.StreamWriteSizeInBytes).Returns(100);
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));

            // Act
            BlobTriggerFunction.Run(mockBlob.Object, "blob", mockLogger.Object);

            // Assert
            mockLogger.Verify(l => l.LogInformation(It.Is<string>(s => s.Contains("Processed blob")), It.IsAny<object[]>()), Times.Once);
        }
    }
}
