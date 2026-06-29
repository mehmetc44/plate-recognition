using PlakaTanima.Persistence;
using PlakaTanima.Infrastructure;
using PlakaTanima.SignalR.Hubs;
using PlakaTanima.SignalR;
using PlakaTanima.WebUI;
using Hangfire;

// --- LOAD ROOT .ENV FILE ---
var rootDir = Directory.GetCurrentDirectory();
while (rootDir != null)
{
    var envPath = Path.Combine(rootDir, ".env");
    if (File.Exists(envPath))
    {
        foreach (var line in File.ReadAllLines(envPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
            }
        }
        break;
    }
    rootDir = Directory.GetParent(rootDir)?.FullName;
}

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceDI(builder.Configuration);
builder.Services.AddInfrastructureDI(builder.Configuration);
builder.Services.AddSignalRDI();
builder.Services.AddWebUIDI(builder.Configuration);


var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseHangfireDashboard();

app.UseAuthorization();


// --- HUB ENDPOINT TANIMLAMASI (YENİ) ---
app.MapHub<PlateHub>("/plateHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- DATABASE MIGRATION & SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PlakaTanima.Persistence.Contexts.AppDbContext>();
    await PlakaTanima.Persistence.Contexts.DbSeeder.SeedAsync(context);
}

app.Run();