using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class SessionInitializerMiddleware
{
    private readonly RequestDelegate _next;

    public SessionInitializerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only clear session if it is new or needs reset
        if (context.Session.GetString("UserUUID") == null)
        {
            // Session not initialized, set default values
            context.Session.SetString("UserUUID", "uuid-doctor-john");
            context.Session.SetString("UserName", "Dr. John");
            context.Session.SetString("UserRole", "Doctor");
        }

        await _next(context);
    }
}

