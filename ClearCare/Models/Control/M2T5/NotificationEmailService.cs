using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ClearCare.Interfaces;

public class NotificationEmailService : INotificationSender
{
    public async Task sendNotification(string email, string phone, string content)
    {
        try
        {
            // Create a new SMTP client for Gmail
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587, // Use 587 for TLS or 465 for SSL
                Credentials = new NetworkCredential("ICT2112TesterP3M2T5@gmail.com", "isdb mgyc hqeo swfr"), // Use your app password here
                EnableSsl = true, // Enable SSL/TLS
            };

            // Create the email message
            MailMessage message = new MailMessage
            {
                From = new MailAddress("example@gmail.com"), // Your Gmail address here
                Subject = "ClearCare Notification", // Email subject
                Body = content, // The email content
                IsBodyHtml = false, // Set to true if your content is HTML
            };

            message.To.Add(email); // Add recipient email

            // Send the email
            await smtpClient.SendMailAsync(message);
            Console.WriteLine("Email sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
