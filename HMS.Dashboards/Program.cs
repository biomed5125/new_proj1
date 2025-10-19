using HMS.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Typed HttpClient to call HMS.Api
builder.Services.AddHttpClient("api", c =>
{
    var apiBase = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7190";
    c.BaseAddress = new Uri(apiBase);
});

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddHmsSdk(builder.Configuration);

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
