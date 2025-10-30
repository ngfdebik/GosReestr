using AuthTest;
using AuthTest.Server;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Добавляем только API-контроллеры
builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
{
    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
});


// Конфигурация JWT настроек
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Регистрация сервисов
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка аутентификации

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings?.Key ?? GenerateRandomKey());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? GenerateRandomKey())),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            // Важно: указываем где искать роли
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeAdmin", policy =>
        policy.RequireRole("Администратор"));

    options.AddPolicy("MustBeLoggedIn", policy =>
        policy.RequireRole("Администратор", "Пользователь"));

    //// Альтернативный вариант через claims (если предпочитаете)
    //options.AddPolicy("MustBeAdminClaim", policy =>
    //    policy.RequireClaim(ClaimTypes.Role, "Администратор"));
});


LinqToDBForEFTools.Initialize();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<DbContextTable>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<DbContextTable>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//LinqToDBForEFTools.Initialize();
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//// Добавьте параметры в connection string если их нет
//var updatedConnectionString = connectionString;
//if (!connectionString.Contains("CommandTimeout"))
//    updatedConnectionString += ";CommandTimeout=300";
//if (!connectionString.Contains("Timeout"))
//    updatedConnectionString += ";Timeout=300";
//if (!connectionString.Contains("Keepalive"))
//    updatedConnectionString += ";Keepalive=30";

//builder.Services.AddDbContextFactory<DbContextTable>(options =>
//    options.UseNpgsql(updatedConnectionString, npgsqlOptions =>
//    {
//        npgsqlOptions.CommandTimeout(300); // 5 минут
//        npgsqlOptions.EnableRetryOnFailure(
//            maxRetryCount: 3,
//            maxRetryDelay: TimeSpan.FromSeconds(5),
//            errorCodesToAdd: null);
//    }));

//builder.Services.AddDbContext<DbContextTable>(options =>
//    options.UseNpgsql(updatedConnectionString, npgsqlOptions =>
//    {
//        npgsqlOptions.CommandTimeout(300);
//        npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
//    }));

//builder.Services.AddScoped<DbContextTable>();
//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//CORS — ОБЯЗАТЕЛЬНО для SPA, если фронт на другом порту (например, localhost:5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDev", policy =>
    {
        policy.WithOrigins("https://localhost:56845") // - порт Vue dev server (AuthTest.Client)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // если используете куки
    });
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 100_000_000; // 100MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000; // 100MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

//Middleware — порядок важен!
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // можно оставить или заменить на JSON-ответ
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; //чтобы Swagger открывался на КОРНЕ: https://localhost:7229/
    });
}

app.UseHttpsRedirection();

//CORS должен быть ДО UseAuthentication и UseAuthorization
app.UseCors("VueDev");

app.UseStaticFiles(); // если хотите отдавать index.html из wwwroot (альтернатива — отдельный хостинг для Vue)

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Только API-маршруты — Razor Pages и MVC Routes УДАЛЕНЫ
app.MapControllers(); // ← основной метод для API

// 🔄 Fallback для SPA — если используете History Mode в Vue Router
// app.MapFallbackToFile("index.html"); // раскомментировать, если Vue билдится в wwwroot

app.Run();

string GenerateRandomKey()
{
    var key = new byte[32];
    RandomNumberGenerator.Fill(key);
    return Convert.ToBase64String(key);
}