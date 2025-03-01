using ClearCare.Models.Interface; 
using ClearCare.Models.Control;   

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

builder.Services.AddScoped<IEmail, EmailService>(); // Ensure EmailService implements IEmail
builder.Services.AddScoped<IPassword, EncryptionManagement>(); // Ensure EncryptionManagement implements IPassword
builder.Services.AddScoped<IEncryption, EncryptionManagement>(); // Ensure EncryptionManagement implements IEncryption
builder.Services.AddScoped<IMedicalRecord, ViewMedicalRecord>(); // Ensure ViewMedicalRecord implements IMedicalRecord
builder.Services.AddScoped<IUserDetails, ProfileManagement>(); // Ensure ProfileManagement implements IUserDetails
builder.Services.AddScoped<ViewMedicalRecord>(); // Register ViewMedicalRecord
builder.Services.AddScoped<ManageMedicalRecord>(); // Register ManageMedicalRecord

var app = builder.Build();

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

// To allow app to use session
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=displayLogin}/{id?}");

app.Run();
