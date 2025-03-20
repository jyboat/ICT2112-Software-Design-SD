using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClearCare.Models.Entities
{
public class NotificationPreference
{
    public string UserID { get; set; }
    public string Preference { get; set; }

    public List<string> Methods { get; set; } // SMS, Email, etc.

    // Constructor to accept both UserID and Preference
    public NotificationPreference(string userId, string preference, List<string> methods)
    {
        UserID = userId;
        Preference = preference;
        Methods = methods;
    }
}

}
