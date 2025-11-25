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
    //options.UseSqlServer(connectionString);
    options.UseNpgsql(connectionString);
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

// LƯU Ý: Render cần biến môi trường ASPNETCORE_ENVIRONMENT = Development để chạy vào đây
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

  
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ShortenerDbContext>();
            // Dòng này gây lỗi sập app (Exit 139) do không kết nối được DB
            context.Database.Migrate(); 
        }
        catch (Exception ex)
        {
            // Ghi log lỗi ra nhưng KHÔNG làm sập app
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Lỗi kết nối Database - Bỏ qua để chạy Web.");
        }
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();