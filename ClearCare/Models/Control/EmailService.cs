using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ClearCare.Models.Interface;

public class EmailService : IEmail
{
    private const string GmailSmtpHost = "smtp.gmail.com";
    private const int GmailSmtpPort = 587;
    private const string SenderEmail = "ICT2112Project@gmail.com"; // Replace with your Gmail
    private const string SenderPassword = "fqiy hzvg gzhz ftqv"; // Use the App Password from Google

    public async Task<bool> sendOTPEmail(string toEmail, string otpCode)
    {
        try
        {
            using (var client = new SmtpClient(GmailSmtpHost, GmailSmtpPort))
            {
                client.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(SenderEmail),
                    Subject = "Your OTP Code",
                    Body = $"Your OTP code is: {otpCode}\n\nThis OTP is valid for 10 minutes.",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}
