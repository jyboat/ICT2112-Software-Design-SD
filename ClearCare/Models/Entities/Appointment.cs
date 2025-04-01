namespace ClearCare.Models.Entities;

// Stub from Module 2
public class Appointment
{
    public Appointment(string id, string title, DateTime timing)
    {
        Id = id;
        Title = title;
        Timing = timing;
    }

    public string Id { get; set; }
    public string Title { get; set; }

    public DateTime Timing { get; set; }
}