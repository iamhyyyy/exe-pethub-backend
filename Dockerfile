# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- SỬA CHỖ NÀY: Copy file .csproj trước để cache NuGet ---
COPY ["EXE_PET_HUB.API/EXE_PET_HUB.API.csproj", "EXE_PET_HUB.API/"]
# Nếu dự án của cậu có các lớp Infrastructure, Domain, Application như bài SmartCarWash, cậu cũng copy các file .csproj của chúng vào đây luôn nhé.

RUN dotnet restore EXE_PET_HUB.API/EXE_PET_HUB.API.csproj

# Sau đó mới copy toàn bộ code còn lại
COPY . .
RUN dotnet publish EXE_PET_HUB.API/EXE_PET_HUB.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 8080

ENTRYPOINT ["dotnet", "EXE_PET_HUB.API.dll"]