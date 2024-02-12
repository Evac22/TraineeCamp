using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

public static class BlobTriggerFunction
{
    [FunctionName("BlobTriggerFunction")]
    public static void Run(
        [BlobTrigger("traineecamp22/{name}", Connection = "BlobConnectionString")] Stream blobStream,
        string name,
        ILogger log,
        ExecutionContext context)
    {
        log.LogInformation($"C# Blob trigger function processed blob\n Name:{name} \n Size: {blobStream.Length} Bytes");

        // TODO: Add code to send notification email
        SendNotificationEmail(name, blobStream, log, context);
    }

    private static void SendNotificationEmail(string fileName, Stream blobStream, ILogger log, ExecutionContext context)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var smtpServer = config["SmtpServer"];
        var smtpPort = int.Parse(config["SmtpPort"]);
        var smtpUsername = config["SmtpUsername"];
        var smtpPassword = config["SmtpPassword"];
        var senderEmail = config["SenderEmail"];
        var recipientEmail = config["RecipientEmail"];

        // Create attachment from blobStream
        var attachment = new MimePart("application", "octet-stream")
        {
            Content = new MimeContent(blobStream),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = fileName
        };

        using (var client = new SmtpClient())
        {
            client.Connect(smtpServer, smtpPort, false);
            client.Authenticate(smtpUsername, smtpPassword);

            SendNotificationWithAttachment(attachment, recipientEmail, senderEmail, client);

            client.Disconnect(true);
        }

        log.LogInformation("Email sent successfully");
    }

    private static void SendNotificationWithAttachment(MimePart attachment, string recipientEmail, string senderEmail, SmtpClient client)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", senderEmail));
        message.To.Add(new MailboxAddress("Recipient Name", recipientEmail));
        message.Subject = "File Upload Notification";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.Attachments.Add(attachment);
        bodyBuilder.TextBody = "File has been successfully uploaded.";

        message.Body = bodyBuilder.ToMessageBody();

        client.Send(message);
    }
}
