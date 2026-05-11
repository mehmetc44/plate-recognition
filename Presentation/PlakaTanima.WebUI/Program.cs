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

var builder = WebApplication.CreateBuilder(args);

// 1. PostgreSQL ve Entity Framework Core Bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Hangfire Konfigürasyonu (Job'ları PostgreSQL'de tutacak)
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    }));

// 3. Hangfire Sunucusunu Başlat (Arka plan iş motoru)
builder.Services.AddHangfireServer();

// MVC Servisleri
builder.Services.AddControllersWithViews();
// 1. Repository Kaydı (Interface'i somut sınıfa bağlıyoruz)
builder.Services.AddScoped<ILprEventRepository, LprEventRepository>();

// 2. Hangfire Job Kaydı
builder.Services.AddScoped<IEventProcessingJob, EventProcessingJob>();

// 3. MediatR Kaydı (Application katmanındaki tüm handler'ları otomatik bulur)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateLprEventCommand).Assembly));

var app = builder.Build();

// HTTP Request Pipeline Yapılandırması
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

// 4. Hangfire Dashboard'u Aktif Et
app.UseHangfireDashboard("/hangfire");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();