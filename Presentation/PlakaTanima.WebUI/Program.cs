using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Persistence.Contexts;
using PlakaTanima.Application.Abstract.Services;
using PlakaTanima.Application.Abstract.Repositories;
using PlakaTanima.Persistence.Repositories;
using PlakaTanima.Application.Abstract.Jobs;
using PlakaTanima.Infrastructure.Jobs;
using PlakaTanima.Application.Features.Commands;
// Yeni eklenen katmanların namespace'leri
using PlakaTanima.SignalR.Hubs;
using PlakaTanima.SignalR.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. PostgreSQL ve Entity Framework Core Bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Hangfire Konfigürasyonu
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    }));

builder.Services.AddHangfireServer();

// --- SİNYAL VE BİLDİRİM SERVİSLERİ (YENİ) ---
builder.Services.AddSignalR();
// Application katmanındaki interface'i, SignalR katmanındaki somut sınıfa bağlıyoruz
builder.Services.AddScoped<IPlateNotificationService, SignalRPlateNotificationService>();

// MVC Servisleri
builder.Services.AddControllersWithViews();

// --- DEPENDENCY INJECTION KAYITLARI ---
builder.Services.AddScoped<ILprEventRepository, LprEventRepository>();
builder.Services.AddScoped<IEventProcessingJob, EventProcessingJob>();

// MediatR Kaydı
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateLprEventCommand).Assembly));

var app = builder.Build();

// HTTP Request Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 4. Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// --- HUB ENDPOINT TANIMLAMASI (YENİ) ---
app.MapHub<PlateHub>("/plateHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();