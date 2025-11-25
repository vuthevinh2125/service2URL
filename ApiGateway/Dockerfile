# Stage 1: Build (Sử dụng SDK 8.0)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file và restore dependencies
# LƯU Ý: Nếu tên thư mục Project của bạn khác "ApiGateway", hãy sửa lại
COPY ["ApiGateway/ApiGateway.csproj", "ApiGateway/"]
RUN dotnet restore "ApiGateway/ApiGateway.csproj"

# Copy tất cả các file còn lại (Bao gồm ocelot.json)
COPY . .

# Build
WORKDIR /src/ApiGateway
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

# Stage 2: Publish (Tạo ra các file runtime cuối cùng)
FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final (Sử dụng ASP.NET Runtime image nhẹ hơn)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy các file đã publish
COPY --from=publish /app/publish .

# Cấu hình cổng nghe (Render mặc định dùng cổng 80)
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Điểm khởi chạy
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
```

### Bước 2: Thêm cấu hình Gateway vào file `.dockerignore`

Bạn cần bảo Docker bỏ qua các file rác khi Build Gateway.

1.  Mở file **`.dockerignore`** (nằm ở thư mục gốc `TinyUrlWebApp`).
2.  Thêm dòng này vào cuối file:

```text
ApiGateway/bin
ApiGateway/obj