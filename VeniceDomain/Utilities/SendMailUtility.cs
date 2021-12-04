using MailKit.Net.Smtp;
using MimeKit;

namespace VeniceDomain.Utilities
{
    public static class SendMailUtility
    {
        public static void SendEmail(
            string smtpServer, 
            int smtpServerPort, 
            string username, 
            string password, 
            string fromName,
            string toName,
            string toEmail,
            string subject, 
            string body, 
            bool isHtml
        )
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect(smtpServer, smtpServerPort, true);
            smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

            smtpClient.Authenticate(username, password);

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(fromName, username));
            msg.To.Add(new MailboxAddress(toName, toEmail));

            msg.Subject = subject;
            msg.Body = new TextPart(isHtml ? "html" : "plain")
            {
                Text = body
            };

            smtpClient.Send(msg);
            smtpClient.Disconnect(true);
        }
    }
}
