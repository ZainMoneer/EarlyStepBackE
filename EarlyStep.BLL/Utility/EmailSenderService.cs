using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace EarlyStep.BLL.Utility;

// IEmailSender  هو  Interface >> ASP.NET Core Identity
// رح نعمل implement للميثود 
// UseDefaultCredentials . هون ما منفعلها عشان انا ابعت الايميل بالتفاصيل الي بدي اياها مش ديفولت 
// gmail SMTP server  >> smtp.gmail.com "port"
public class EmailSenderService : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("zeenjaber12@gmail.com", "isva vyzz txch nfxc"),
            EnableSsl = true
        };

        return client.SendMailAsync(
            new MailMessage(
                from: "zeenjaber12@gmail.com",
                to: email,
                subject,
                message
            )
            {
                IsBodyHtml = true
            }
            );
    }
}



