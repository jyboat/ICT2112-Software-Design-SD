using ClearCare.Controls;
using ClearCare.DataSource.M3T2;
using ClearCare.Models.Control.M3T2;
using ClearCare.Models.Interfaces.M3T2;
using Microsoft.AspNetCore.Http;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Set Google Application Credentials globally
string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "ict2112-firebase-adminsdk-fbsvc-75dd74a153.json");
System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Register distributed cache and session services.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// **Register IHttpContextAccessor**
builder.Services.AddHttpContextAccessor();

// Register your gateway and control services.
// Note: Since PatientDrugMapper depends on IHttpContextAccessor (per-request data), consider using Scoped
// instead of Singleton if needed. For now, we'll keep it as Singleton after adding IHttpContextAccessor.
builder.Services.AddSingleton<EnquiryGateway>();
builder.Services.AddSingleton<EnquiryControl>();

builder.Services.AddSingleton<SideEffectsMapper>();
builder.Services.AddScoped<SideEffectControl>();

// Remove duplicate registration of PatientDrugLogControl if exists.
builder.Services.AddScoped<PatientDrugLogControl>();

builder.Services.AddScoped<IFetchPrescriptions, PrescriptionControl>();
builder.Services.AddSingleton<PrescriptionMapper>();
builder.Services.AddScoped<PrescriptionControl>();

builder.Services.AddHttpClient<IFetchSideEffects, DrugLogSideEffectsService>();

builder.Services.AddSingleton<PatientDrugMapper>(); // Now this can resolve IHttpContextAccessor.
builder.Services.AddSingleton<DrugLogSideEffectsService>();
// Remove duplicate registration of PatientDrugLogControl if any.
builder.Services.AddScoped<DrugInteractionControl>();

builder.Services.AddScoped<UserSwitcherService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var enquiryControl = scope.ServiceProvider.GetRequiredService<EnquiryControl>();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.UseMiddleware<SessionInitializerMiddleware>();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
