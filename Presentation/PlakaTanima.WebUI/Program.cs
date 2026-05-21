using PlakaTanima.Persistence;
using PlakaTanima.Infrastructure;
using PlakaTanima.SignalR.Hubs;
using PlakaTanima.WebUI.HostedServices;
using PlakaTanima.SignalR;
using PlakaTanima.WebUI;
using PlakaTanima.Alarm;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceDI(builder.Configuration);
builder.Services.AddInfrastructureDI(builder.Configuration);
builder.Services.AddSignalRDI();
builder.Services.AddWebUIDI(builder.Configuration);
builder.Services.AddAlarmDI();


var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// --- HUB ENDPOINT TANIMLAMASI (YENİ) ---
app.MapHub<PlateHub>("/plateHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();