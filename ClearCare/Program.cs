using ClearCare.Domain;
using ClearCare.Gateways;
using ClearCare.Observers;

var builder = WebApplication.CreateBuilder(args);

// Set Google Application Credentials globally
string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "ict2112-firebase-adminsdk-fbsvc-75dd74a153.json");
System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEnquiryGateway, EnquiryGateway>();

// Register EnquirySubject
builder.Services.AddSingleton<EnquirySubject>();

// Register observers for Enquiry
builder.Services.AddSingleton<IEnquiryObserver, LoggingObserver>();
builder.Services.AddSingleton<IEnquiryObserver, NotificationObserver>();

// Register SideEffectSubject
builder.Services.AddSingleton<SideEffectSubject>();

// Register SideEffectsMapper and SideEffectControl
builder.Services.AddSingleton<SideEffectsMapper>();
builder.Services.AddScoped<SideEffectControl>();

// Register observers for SideEffect
builder.Services.AddSingleton<ISideEffectObserver, LoggingSideEffectObserver>();

var app = builder.Build();


// Attach observers to the SideEffectSubject
var sideEffectSubject = app.Services.GetRequiredService<SideEffectSubject>();
var loggingObserver = app.Services.GetRequiredService<ISideEffectObserver>();
sideEffectSubject.Attach(loggingObserver);

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
