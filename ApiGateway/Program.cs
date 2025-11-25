using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Ocelot để đọc file định tuyến ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 2. Thêm dịch vụ Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

// 3. Không dùng Swagger/Controllers/HTTPS/Authorization (chỉ định tuyến)
// app.UseSwagger();
// app.UseSwaggerUI();
// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();

// 4. Kích hoạt Ocelot Middleware (Bắt buộc phải là await và đặt ở cuối)
await app.UseOcelot();

app.Run();
