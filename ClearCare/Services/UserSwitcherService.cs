public class UserSwitcherService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSwitcherService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SwitchUser(string userIdentifier)
    {
        var context = _httpContextAccessor.HttpContext;
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

    public Dictionary<string, string> GetAvailableUsers()
    {
        return new Dictionary<string, string>
        {
            { "uuid-doctor-john", "Dr. John (Doctor)" },
            { "uuid-patient-sara", "Sara (Patient)" },
            { "uuid-patient-john", "John (Patient)" }
        };
    }

    public string GetCurrentUserIdentifier()
    {
        return _httpContextAccessor.HttpContext.Session.GetString("UserID") 
               ?? "uuid-doctor-john"; 
    }
}