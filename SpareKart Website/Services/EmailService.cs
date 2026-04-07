using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace SpareKart_Website.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOtpEmail(string toEmail, string otp, string subject = "Your OTP from SpareKart")
        {
            var senderEmail = _config["SmtpSettings:SenderEmail"];
            var senderName = _config["SmtpSettings:SenderName"];
            var password = _config["SmtpSettings:Password"];
            var host = _config["SmtpSettings:Server"];
            var portString = _config["SmtpSettings:Port"];
            var port = string.IsNullOrEmpty(portString) ? 587 : int.Parse(portString);

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password) || password == "your-app-password")
            {
                // Fallback for development if they haven't set up SMTP yet
                System.Diagnostics.Debug.WriteLine($"============ DEV EMAIL OTP for {toEmail}: {otp} ============");
                return; 
            }

            var message = new MailMessage();
            message.From = new MailAddress(senderEmail, senderName);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = $"Your One Time Password (OTP) is: {otp}\n\nIt is valid for 10 minutes.";
            message.IsBodyHtml = false;

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(senderEmail, password);
                try
                {
                    client.Send(message);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Email send failed: {ex.Message}");
                }
            }
        }
    }
}
