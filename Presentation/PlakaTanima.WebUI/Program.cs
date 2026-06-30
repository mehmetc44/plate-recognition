using PlakaTanima.Persistence;
using PlakaTanima.Infrastructure;
using PlakaTanima.SignalR.Hubs;
using PlakaTanima.SignalR;
using PlakaTanima.WebUI;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

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

// --- AUTOMAPPER CONFIGURATION ---
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new PlakaTanima.Application.Mapping.MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// --- IDENTITY & JWT CONFIGURATION ---
builder.Services.AddIdentity<Microsoft.AspNetCore.Identity.IdentityUser, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<PlakaTanima.Persistence.Contexts.AppDbContext>()
.AddDefaultTokenProviders();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "PlatarSecretSecurityKeyThatNeedsToBeLongEnoughForHMACSHA256";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "PlatarIssuer";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "PlatarAudience";
var key = System.Text.Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key)
    };
});


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

app.UseAuthentication();
app.UseAuthorization();


// --- HUB ENDPOINT TANIMLAMASI (YENİ) ---
app.MapHub<PlateHub>("/plateHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

// --- DATABASE MIGRATION & SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PlakaTanima.Persistence.Contexts.AppDbContext>();
    await PlakaTanima.Persistence.Contexts.DbSeeder.SeedAsync(context);
}

app.Run();