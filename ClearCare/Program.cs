using ClearCare.Controls;
using ClearCare.Gateways;
using ClearCare.Observers;
using ClearCare.Interfaces; 

var builder = WebApplication.CreateBuilder(args);

// Set Google Application Credentials globally.
string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "ict2112-firebase-adminsdk-fbsvc-75dd74a153.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register IHttpContextAccessor (required for session access in services)
builder.Services.AddHttpContextAccessor();

// Register your gateway and control services.
builder.Services.AddSingleton<EnquiryGateway>();
builder.Services.AddSingleton<EnquiryControl>();
builder.Services.AddSingleton<EnquiryLoggingObserver>(); // hypothetical observer

builder.Services.AddSingleton<SideEffectsMapper>();
builder.Services.AddScoped<SideEffectControl>();

builder.Services.AddSingleton<PatientDrugMapper>();
builder.Services.AddScoped<PatientDrugLogControl>();
builder.Services.AddScoped<DrugInteractionControl>();

builder.Services.AddSingleton<PrescriptionMapper>();
builder.Services.AddScoped<PrescriptionControl>();

var app = builder.Build();



// Create a scope to resolve services and attach observers.
using (var scope = app.Services.CreateScope())
{
    var enquiryControl = scope.ServiceProvider.GetRequiredService<EnquiryControl>();
    var loggingObserver = scope.ServiceProvider.GetRequiredService<EnquiryLoggingObserver>();

    // Attach the observer to the enquiry control.
    enquiryControl.Attach(loggingObserver);
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

// Enable session before authentication/authorization.
app.UseSession();

app.UseAuthorization();

app.UseMiddleware<SessionInitializerMiddleware>();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
