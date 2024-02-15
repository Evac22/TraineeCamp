using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net;
using MailKit.Net.Proxy;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using TraineeCamp.AzureFunction;


namespace FunctionApp2
{
    public static class BlobTriggerCSharp
    {
        [FunctionName("BlobTriggerCSharp")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log,
        ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<FileEmailDto>(requestBody);

            var configuration = GetConfiguration(context);
            var emailSettings = GetEmailSettings(configuration);
            var emailMessage = CreateEmailMessage(emailSettings, data.Email, data.FileUri, "");
            await SendMessageAsync(emailMessage, emailSettings);

            return new OkResult();
        }

        private static async Task SendMessageAsync(MimeMessage message, EmailSettings emailSettings)
        {
            using var smtpClient = new SmtpClient();
            try
            {
                await smtpClient.ConnectAsync(emailSettings.SmtpServer, emailSettings.Port, true);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                await smtpClient.AuthenticateAsync(emailSettings.Username, emailSettings.Password);
                await smtpClient.SendAsync(message);
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }
        }

        private static MimeMessage CreateEmailMessage(EmailSettings emailSettings, string email, string uri, string token)
        {
            var emailMessage = new MimeMessage
            {
                Subject = "Your file is successfully uploaded to Azure Blob Storage!"
            };
            emailMessage.From.Add(new MailboxAddress("NotificationFromAzureFunc", emailSettings.From));
            emailMessage.To.Add(new MailboxAddress("receiver", email));
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = $"Your docx file was uploaded, here is the URL with SAS token: {uri}"
            };

            return emailMessage;
        }


        private static EmailSettings GetEmailSettings(IConfiguration config) =>
      new(
          config["MY_EMAIL"],
          "smtp.gmail.com",
          465,
          config["MY_EMAIL"],
          config["REENBIT_EMAIL_APP_PASS"]
      );

        private static IConfiguration GetConfiguration(ExecutionContext context) =>
            new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddEnvironmentVariables()
                .Build();
    }

}
