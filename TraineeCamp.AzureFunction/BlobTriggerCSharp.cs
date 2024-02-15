using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

public static class BlobTriggerCSharp
{
    [FunctionName("BlobTriggerCSharp")]
    public static void Run([BlobTrigger("traineecamp22/{name}", Connection = "settingConnection")] Stream myBlob, string name, ILogger log)
    {
        log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

        var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("settingConnection"));
        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference("traineecamp22");
        var blob = container.GetBlockBlobReference(name);

        var sasConstraints = new SharedAccessBlobPolicy
        {
            SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
            Permissions = SharedAccessBlobPermissions.Read
        };

        var sasToken = blob.GetSharedAccessSignature(sasConstraints);
        var blobUrl = blob.Uri + sasToken;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 465,
            Credentials = new NetworkCredential("chubarov205@gmail.com", "ajia jign wubi ubqz"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("chubarov205@gmail.com"),
            Subject = "Уведомление о загрузке файла",
            Body = $"Файл успешно загружен. URL файла: {blobUrl}",
        };

        mailMessage.To.Add("recipient-email@example.com");

        smtpClient.Send(mailMessage);
    }
}
