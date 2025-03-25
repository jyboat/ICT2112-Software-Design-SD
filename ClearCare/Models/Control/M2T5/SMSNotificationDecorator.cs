using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClearCare.Interfaces;

public class SMSNotificationDecorator : NotificationDecorator
{
    private readonly string _apiKey = "example"; // Replace with your SMS.to API key
    private readonly string _endpoint = "https://api.sms.to/sms/send";

    public SMSNotificationDecorator(INotificationSender notificationSender)
        : base(notificationSender)
    {
    }

    public override async Task sendNotification(string email, string phone, string content)
    {
        // Send email notification
        await base.sendNotification(email, phone, content);

        // Send SMS notification
        await sendSMSNotification(phone, content);
    }

    private async Task sendSMSNotification(string phone, string content)
    {
        using (HttpClient client = new HttpClient())
        {
            // Set the Authorization header with the Bearer token.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Create the JSON payload.
            var payload = new
            {
                message = content,
                to = phone,
                bypass_optout = true,
                sender_id = "ClearCare", // Customize your sender ID if needed
            };

            // Serialize payload to JSON.
            string jsonPayload = JsonSerializer.Serialize(payload);

            // Create HTTP content.
            HttpContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Send the POST request.
            HttpResponseMessage response = await client.PostAsync(_endpoint, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("[NotificationSMSService] SMS sent successfully.");
            }
            else
            {
                Console.WriteLine($"[NotificationSMSService] Error sending SMS: {responseContent}");
            }
        }
    }
}