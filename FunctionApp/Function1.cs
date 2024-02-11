using MailKit.Net.Smtp;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MimeKit;


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
        SendNotificationEmail(name, log, context);
    }

    private static void SendNotificationEmail(string fileName, ILogger log, ExecutionContext context)
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

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", senderEmail));
        message.To.Add(new MailboxAddress("Recipient Name", recipientEmail));
        message.Subject = "File Upload Notification";

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.TextBody = "File has been successfully uploaded.";
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.Connect(smtpServer, smtpPort, false);
            client.Authenticate(smtpUsername, smtpPassword);
            client.Send(message);
            client.Disconnect(true);
        }

        log.LogInformation("Email sent successfully");
    }
}
