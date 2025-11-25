// ShortenUrl/Data/ShortenerDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ShortenUrl.Data
{
    public class ShortenerDbContextFactory : IDesignTimeDbContextFactory<ShortenerDbContext>
    {
        public ShortenerDbContext CreateDbContext(string[] args)
        {
            // Lấy đường dẫn cơ sở là thư mục làm việc hiện tại (Project Directory)
            var basePath = Directory.GetCurrentDirectory();

            // 1. Khởi tạo Configuration
            // PMC sẽ đọc appsettings.json TỪ THƯ MỤC CỦA PROJECT
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Lấy Connection String
            var connectionString = configuration.GetConnectionString("ShortenerDbConnection");

            // 3. Tạo DbContextOptions thủ công
            var optionsBuilder = new DbContextOptionsBuilder<ShortenerDbContext>();

            // Dùng Connection String đã được đọc từ appsettings.json
            optionsBuilder.UseSqlServer(connectionString);

            return new ShortenerDbContext(optionsBuilder.Options);
        }
    }
}