using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

public class UserSwitcherService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///   Initializes a new instance of the <see cref="UserSwitcherService"/>
    ///   class.
    /// </summary>
    /// <param name="httpContextAccessor">
    ///   The HttpContextAccessor for accessing session data.
    /// </param>
    public UserSwitcherService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///   Switches the current user session to the specified user.
    /// </summary>
    /// <param name="userIdentifier">
    ///   The identifier of the user to switch to.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   Thrown if there is no HttpContext available or if the session is not
    ///   available or not configured.
    /// </exception>
    public void SwitchUser(string userIdentifier)
    {
        var context = _httpContextAccessor.HttpContext;
        // If there's no HttpContext, we can't proceed with session usage
        if (context == null)
        {
            throw new InvalidOperationException(
                "No HttpContext is available to switch users."
            );
        }

        // Similarly, the session may be null if there's no session middleware
        // or it's not yet started
        if (context.Session == null)
        {
            throw new InvalidOperationException(
                "Session is not available or not configured."
            );
        }

        // Clear existing session data
        context.Session.Clear();

        switch (userIdentifier)
        {
            case "uuid-doctor-john":
                context.Session.SetString("UserID", "uuid-doctor-john");
                context.Session.SetString("Name", "Dr. John");
                context.Session.SetString("Role", "Doctor");
                break;
            case "uuid-patient-sara":
                context.Session.SetString("UserID", "uuid-patient-sara");
                context.Session.SetString("Name", "Sara");
                context.Session.SetString("Role", "Patient");
                break;
            case "uuid-patient-john":
                context.Session.SetString("UserID", "uuid-patient-john");
                context.Session.SetString("Name", "John");
                context.Session.SetString("Role", "Patient");
                break;
            default:
                context.Session.SetString("UserID", "uuid-doctor-john");
                context.Session.SetString("Name", "Dr. John");
                context.Session.SetString("Role", "Doctor");
                break;
        }
    }

    /// <summary>
    ///   Gets a dictionary of available users and their display names.
    /// </summary>
    /// <returns>
    ///   A dictionary where the key is the user identifier and the value is
    ///   the display name.
    /// </returns>
    public Dictionary<string, string> GetAvailableUsers()
    {
        return new Dictionary<string, string>
        {
            { "uuid-doctor-john", "Dr. John (Doctor)" },
            { "uuid-patient-sara", "Sara (Patient)" },
            { "uuid-patient-john", "John (Patient)" }
        };
    }

    /// <summary>
    ///   Gets the identifier of the currently logged-in user.
    /// </summary>
    /// <returns>
    ///   The user identifier, or "uuid-doctor-john" if no user is currently
    ///   logged in.
    /// </returns>
    public string GetCurrentUserIdentifier()
    {
        // Safely navigate to the session, then get the string. If anything is
        // null or the key is absent, return the default.
        return _httpContextAccessor.HttpContext?.Session?.GetString("UserID") ??
            "uuid-doctor-john";
    }
}
