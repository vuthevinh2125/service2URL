// ShortenUrl/Program.cs 
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using ShortenUrl.Data;
using ShortenUrl.Interfaces;
using ShortenUrl.Repositories;
using ShortenUrl.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ SERVICES ---

// Đọc Connection String từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ShortenerDbConnection");

// Đăng ký DbContext (Sử dụng SQL Server)
builder.Services.AddDbContext<ShortenerDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Đăng ký Repository (Scoped) và Services (Singleton)
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddSingleton<ShortCodeGenerator>();

// Cấu hình API cơ bản
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 2. CẤU HÌNH HTTP PIPELINE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Áp dụng Migration lúc khởi động (Lab 4, Lab 8)
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ShortenerDbContext>();
        context.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();