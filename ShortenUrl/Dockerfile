# Stage 1: Build & Test (Sử dụng SDK image)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy Solution File (Đã sửa lỗi context)
COPY ["ShortenUrl/ShortenUrl.csproj", "ShortenUrl/"] 
COPY ["ShortenUrl.Tests/ShortenUrl.Tests.csproj", "ShortenUrl.Tests/"] 

# RUN dotnet restore DÙNG CÚ PHÁP MẢNG
RUN dotnet restore ShortenUrl/ShortenUrl.csproj 
RUN dotnet restore ShortenUrl.Tests/ShortenUrl.Tests.csproj
# Copy code còn lại và chạy Test/Build
COPY . .

# Chạy Unit Tests
RUN ["dotnet", "test", "ShortenUrl.Tests/ShortenUrl.Tests.csproj", "--no-restore"] 
# Build Project chính
RUN ["dotnet", "build", "ShortenUrl/ShortenUrl.csproj", "-c", "Release", "-o", "/app/build", "--no-restore"]

# Publish (Tạo ra các file runtime cuối cùng)
FROM build AS publish
WORKDIR /src/ShortenUrl
RUN ["dotnet", "publish", "ShortenUrl.csproj", "-c", "Release", "-o", "/app/publish", "/p:UseAppHost=false"]

# Stage 2: Final (Sử dụng ASP.NET Runtime image nhẹ hơn)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy các file đã publish
COPY --from=publish /src/ShortenUrl/app/publish .

# Cấu hình cổng nghe
ENV ASPNETCORE_URLS=http://+:80 
EXPOSE 80

# Điểm khởi chạy
ENTRYPOINT ["dotnet", "ShortenUrl.dll"]