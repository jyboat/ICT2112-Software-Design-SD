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
        if (context.Session.GetString("UserID") == null)
        {
            // Session not initialized, set default values
            context.Session.SetString("UserID", "uuid-doctor-john");
            context.Session.SetString("Name", "Dr. John");
            context.Session.SetString("Role", "Doctor");
        }

        await _next(context);
    }
}

