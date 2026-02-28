# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore EXE_PET_HUB.API/EXE_PET_HUB.API.csproj
RUN dotnet publish EXE_PET_HUB.API/EXE_PET_HUB.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:$PORT

EXPOSE 8080

ENTRYPOINT ["dotnet", "EXE_PET_HUB.API.dll"]