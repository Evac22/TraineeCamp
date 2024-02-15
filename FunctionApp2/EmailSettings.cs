

namespace TraineeCamp.AzureFunction
{
    public record EmailSettings(
     string From,
     string SmtpServer,
     int Port,
     string Username,
     string Password
    );
}
