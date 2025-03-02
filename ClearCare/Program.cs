using ClearCare.Models.Interface; 
using ClearCare.Models.Control;   
using ClearCare.Models.Hubs;   
using ClearCare.Controllers;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 🔹 Session timeout (adjust as needed)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add SignalR to the DI container
builder.Services.AddSignalR();  

// Register MedRecordSubject as Singleton using its Interface
builder.Services.AddSingleton<IMedicalRecordSubject, MedRecordSubject>();

// Register UpdateViewObserver AFTER MedRecordSubject
builder.Services.AddSingleton<UpdateViewObserver>();


builder.Services.AddScoped<IEmail, EmailService>(); // Ensure EmailService implements IEmail
builder.Services.AddScoped<IPassword, EncryptionManagement>(); // Ensure EncryptionManagement implements IPassword
builder.Services.AddScoped<IEncryption, EncryptionManagement>(); // Ensure EncryptionManagement implements IEncryption
builder.Services.AddScoped<IMedicalRecord, ViewMedicalRecord>(); // Ensure ViewMedicalRecord implements IMedicalRecord
builder.Services.AddScoped<IUserDetails, ProfileManagement>(); // Ensure ProfileManagement implements IUserDetails

var app = builder.Build();

// // Ensure UpdateViewObserver is created and added to the ObserverManager
// var medRecordSubject = app.Services.GetRequiredService<MedRecordSubject>();
// var hubContext = app.Services.GetRequiredService<IHubContext<MedicalRecordHub>>();
// new UpdateViewObserver(hubContext, medRecordSubject);  // Manually instantiate

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map the SignalR Hub to the "/medicalRecordHub" URL
app.MapHub<MedicalRecordHub>("/medicalRecordHub");

// To allow app to use session
app.UseSession();

// Required services
// Start UpdateViewObserver automatically (ensures observer is created)
app.Services.GetRequiredService<UpdateViewObserver>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=displayLogin}/{id?}");

app.Run();
