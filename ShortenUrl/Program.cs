using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ShortenUrl.Data;
using ShortenUrl.Services;
using ShortenUrl.Interfaces;
using ShortenUrl.Repositories;
using ShortenUrl.Clients; // <--- Cần thêm namespace này
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------
// 1. KẾT NỐI DATABASE (Service 2 - ShortenUrl)
// -----------------------------------------------------------------

var connectionString = builder.Configuration.GetConnectionString("ShortenerDbConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("FATAL: Connection string is missing. Please check the 'ConnectionStrings__ShortenerDbConnection' variable on Render.");
}

builder.Services.AddDbContext<ShortenerDbContext>(options =>
{
    // Sử dụng PostgreSQL
    options.UseNpgsql(connectionString);
});

// -----------------------------------------------------------------
// 2. KẾT NỐI SANG SERVICE 3 (UserManagement)
// -----------------------------------------------------------------

// 👇 ĐĂNG KÝ HTTP CLIENT ĐỂ GỌI SANG USER MANAGEMENT SERVICE
builder.Services.AddHttpClient("UserManagementService", client =>
{
    // URL của Service 3 (Đã cập nhật theo link mới nhất bạn cung cấp)
    client.BaseAddress = new Uri("https://userservice-latest-p29g.onrender.com/");
});

// 👇 ĐĂNG KÝ USER MANAGEMENT CLIENT
// (Bạn cần tạo IUserManagementClient và UserManagementClient trong thư mục Clients)
builder.Services.AddScoped<IUserManagementClient, UserManagementClient>();
// 👆 -----------------------------------------------------------------

// Đăng ký Service và Repository
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddSingleton<ShortCodeGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Cấu hình Swagger/OpenAPI Security (Giữ nguyên)
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập token Admin vào đây"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "1234567890qwertyuiopgsdgsdgsdgsdgsdgsdgsdgdsgsdgsdgsdgdsgsdgsdgdsgsdrewwetwetewtwetewtewtwetwetwetewweewrwererwerwerewrwerwerwerwe";

// Cấu hình JWT Authentication (Giữ nguyên)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://your-issuer.com",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "https://your-audience.com",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

// Cấu hình CORS (Để cho phép Frontend gọi vào)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
      policy =>
      {
          // Cho phép Frontend (Service 1) và Service 3 gọi vào (nếu cần)
          policy.WithOrigins(
              "https://fe-render.onrender.com",
              "https://userservice-latest-p29g.onrender.com", // Đã thêm Service 3 vào CORS
              "http://localhost:3000",
              "http://localhost:5173"
          )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
      });
});

var app = builder.Build();

// -----------------------------------------------------------------
// 3. TỰ ĐỘNG TẠO BẢNG (Migration)
// -----------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ShortenerDbContext>();
        context.Database.Migrate();
        Console.WriteLine("--> Database migrated successfully for ShortenUrl!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("--> Error during Migration for ShortenUrl: " + ex.Message);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();