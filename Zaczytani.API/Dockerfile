# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Zaczytani.API/Zaczytani.API.csproj", "Zaczytani.API/"]
COPY ["Zaczytani.Application/Zaczytani.Application.csproj", "Zaczytani.Application/"]
COPY ["Zaczytani.Domain/Zaczytani.Domain.csproj", "Zaczytani.Domain/"]
COPY ["Zaczytani.Infrastructure/Zaczytani.Infrastructure.csproj", "Zaczytani.Infrastructure/"]
RUN dotnet restore "./Zaczytani.API/Zaczytani.API.csproj"
COPY . .
WORKDIR "/src/Zaczytani.API"
RUN dotnet build "./Zaczytani.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Zaczytani.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zaczytani.API.dll"]