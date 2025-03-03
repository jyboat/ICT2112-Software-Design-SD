using ClearCare.Controls;
using ClearCare.Gateways;
using ClearCare.Observers;

var builder = WebApplication.CreateBuilder(args);

/// where to put this - > // Set Google Application Credentials globally
string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "ict2112-firebase-adminsdk-fbsvc-75dd74a153.json");
System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Register your gateway and control
builder.Services.AddSingleton<EnquiryGateway>(); 
builder.Services.AddSingleton<EnquiryControl>(); 
builder.Services.AddSingleton<EnquiryLoggingObserver>(); // hypothetical observer
builder.Services.AddSingleton<SideEffectGateway>();
builder.Services.AddSingleton<SideEffectControl>();

var app = builder.Build();

// Create a scope to resolve services
using (var scope = app.Services.CreateScope())
{
    var enquiryControl = scope.ServiceProvider.GetRequiredService<EnquiryControl>();
    var loggingObserver = scope.ServiceProvider.GetRequiredService<EnquiryLoggingObserver>();

    // Attach the observer
    enquiryControl.Attach(loggingObserver);
}


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


