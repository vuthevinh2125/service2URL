# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file csproj và restore
COPY ["ShortenUrl/ShortenUrl.csproj", "ShortenUrl/"]
COPY ["ShortenUrl.Tests/ShortenUrl.Tests.csproj", "ShortenUrl.Tests/"]

RUN dotnet restore "ShortenUrl/ShortenUrl.csproj"
RUN dotnet restore "ShortenUrl.Tests/ShortenUrl.Tests.csproj"

# Copy toàn bộ source code
COPY . .

# Build
WORKDIR /src/ShortenUrl
RUN dotnet build "ShortenUrl.csproj" -c Release -o /app/build

# Publish (Xuất file ra thư mục /app/publish)
FROM build AS publish
RUN dotnet publish "ShortenUrl.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# --- SỬA Ở ĐÂY: ---
# Copy từ /app/publish (nơi vừa xuất ra) vào thư mục hiện tại (.)
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ShortenUrl.dll"]