using ClearCare.Models.Interface; 
using ClearCare.Models.Control;   
using ClearCare.Models.Hubs;   
using ClearCare.Controllers;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.UseUrls("http://10.132.18.96:5007");

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 🔹 Session timeout (adjust as needed)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Enable CORS with specific policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5007", "http://10.132.18.96:5007")  // Add user A and B's IP
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add SignalR to the DI container
builder.Services.AddSignalR();  

builder.Services.AddScoped<IEmail, EmailService>(); // Ensure EmailService implements IEmail
builder.Services.AddScoped<IPassword, EncryptionManagement>(); // Ensure EncryptionManagement implements IPassword
builder.Services.AddScoped<IEncryption, EncryptionManagement>(); // Ensure EncryptionManagement implements IEncryption
builder.Services.AddScoped<IMedicalRecord, ViewMedicalRecord>(); // Ensure ViewMedicalRecord implements IMedicalRecord
builder.Services.AddScoped<IUserDetails, ProfileManagement>(); // Ensure ProfileManagement implements IUserDetails
builder.Services.AddScoped<IMedicalRecordSubject, ManageMedicalRecord>();
builder.Services.AddScoped<UpdateViewObserver>();

var app = builder.Build();

// // Ensure UpdateViewObserver is created and added to the ObserverManager
using (var scope = app.Services.CreateScope())
{
    var observer = scope.ServiceProvider.GetRequiredService<UpdateViewObserver>();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use the CORS policy
app.UseCors("AllowSpecificOrigin");

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=displayLogin}/{id?}");

app.Run();
