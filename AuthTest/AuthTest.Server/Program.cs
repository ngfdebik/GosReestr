using AuthTest.Server;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// УДАЛИЛИ Razor Pages и MVC Views — оставили только API
// builder.Services.AddRazorPages()-УДАЛЕНО
// builder.Services.AddMvc().AddRazorPagesOptions-УДАЛЕНО

//Добавляем только API-контроллеры
builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
{
    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
});

//Аутентификация (оставляем куки, но теперь для API — лучше перейти на JWT в будущем)
builder.Services.AddAuthentication().AddCookie("EGRCookieAuth", options =>
{
    options.Cookie.Name = "EGRCookieAuth";
    // Важно для SPA: разрешить доступ с фронтенда (если фронт на другом порте/домене)
    options.Cookie.SameSite = SameSiteMode.None; // или Lax, если фронт на том же домене
    options.Cookie.HttpOnly = false; // если Vue.js должен читать куки (не рекомендуется) — лучше HttpOnly + флаг в теле ответа
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // только по HTTPS
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Авторизация — оставляем как есть (работает и с API)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeAdmin", policy => policy.RequireClaim("UserType", "Администратор"));
    options.AddPolicy("MustBeLoggedIn", policy => policy.RequireClaim("UserType", new string[] { "Администратор", "Пользователь" }));
});

//База данных — без изменений
LinqToDBForEFTools.Initialize();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<DbContextTable>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<DbContextTable>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//CORS — ОБЯЗАТЕЛЬНО для SPA, если фронт на другом порту (например, localhost:5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // - порт Vue dev server (AuthTest.Client)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // если используете куки
    });
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