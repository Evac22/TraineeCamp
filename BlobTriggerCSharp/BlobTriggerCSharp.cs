using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Mail;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using System.IO;

public static class BlobTriggerCSharp
{
    [FunctionName("BlobTriggerCSharp")]
    public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
    {
        log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

        // Create a SAS token that's valid for one hour.
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference("samples-workitems");
        CloudBlockBlob blob = container.GetBlockBlobReference(name);

        SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
        {
            SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1),
            Permissions = SharedAccessBlobPermissions.Read
        };

        string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

        // Send an email to the user
        var from = Environment.GetEnvironmentVariable("FromEmail");
        var to = Environment.GetEnvironmentVariable("ToEmail");
        var subject = "File uploaded successfully";
        var body = $"Your file has been uploaded successfully. You can download it using this link: https://your-storage-account.blob.core.windows.net/samples-workitems/{name}{sasBlobToken}";

        var smtpClient = new SmtpClient(Environment.GetEnvironmentVariable("SmtpServer"))
        {
            Port = int.Parse(Environment.GetEnvironmentVariable("SmtpPort")),
            Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("SmtpUsername"), Environment.GetEnvironmentVariable("SmtpPassword")),
            EnableSsl = true,
        };

        smtpClient.Send(from, to, subject, body);
    }
}
