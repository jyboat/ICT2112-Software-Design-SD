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
                context.Session.SetString("UserUUID", "uuid-doctor-john");
                context.Session.SetString("UserName", "Dr. John");
                context.Session.SetString("UserRole", "Doctor");
                break;
                
            case "uuid-patient-sara":
                context.Session.SetString("UserUUID", "uuid-patient-sara");
                context.Session.SetString("UserName", "Sara");
                context.Session.SetString("UserRole", "Patient");
                break;
                
            case "uuid-patient-john":
                context.Session.SetString("UserUUID", "uuid-patient-john");
                context.Session.SetString("UserName", "John");
                context.Session.SetString("UserRole", "Patient");
                break;
                
            default:
                context.Session.SetString("UserUUID", "uuid-doctor-john");
                context.Session.SetString("UserName", "Dr. John");
                context.Session.SetString("UserRole", "Doctor");
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
        return _httpContextAccessor.HttpContext.Session.GetString("UserUUID") 
               ?? "uuid-doctor-john"; 
    }
}