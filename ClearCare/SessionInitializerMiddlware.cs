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
        // Clear any existing session keys if needed.
        context.Session.Clear();

        // Set the session keys with your desired values.
        context.Session.SetString("UserUUID", "uuid-1234");
        context.Session.SetString("UserName", "Alice");
        // context.Session.SetString("UserRole", "Patient");
        context.Session.SetString("UserRole", "Doctor");

        await _next(context);
    }
}
