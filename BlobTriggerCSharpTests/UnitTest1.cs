using System;
using System.IO;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs;
using FunctionApp2;
using Microsoft.WindowsAzure.Storage.Blob;

public class BlobTriggerCSharpTests
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IBinder> _binderMock;
    private readonly Mock<BlobTriggerCSharp> _functionMock;

    public BlobTriggerCSharpTests()
    {
        _loggerMock = new Mock<ILogger>();
        _configMock = new Mock<IConfiguration>();
        _binderMock = new Mock<IBinder>();
        _functionMock = new Mock<BlobTriggerCSharp>(_configMock.Object) { CallBase = true };
    }

    [Fact]
    public void Run_ProcessesBlob_CallsSendEmail()
    {
        // Arrange
        var blobStream = new MemoryStream();
        var blobName = "testblob";

        _functionMock.Setup(f => f.SendEmail(It.IsAny<string>()));

        // Act
        _functionMock.Object.Run(blobStream, blobName, _loggerMock.Object, _binderMock.Object);

        // Assert
        _functionMock.Verify(f => f.SendEmail(It.IsAny<string>()), Times.Once);
    }
}
