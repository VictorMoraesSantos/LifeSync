# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Gym/Gym.API/Gym.API.csproj", "Services/Gym/Gym.API/"]
COPY ["Services/Gym/Gym.Infrastructure/Gym.Infrastructure.csproj", "Services/Gym/Gym.Infrastructure/"]
COPY ["Services/Gym/Gym.Application/Gym.Application.csproj", "Services/Gym/Gym.Application/"]
COPY ["Core/Core.Application/Core.Application.csproj", "Core/Core.Application/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/Gym/Gym.Domain/Gym.Domain.csproj", "Services/Gym/Gym.Domain/"]
COPY ["Core/Core.Domain/Core.Domain.csproj", "Core/Core.Domain/"]
RUN dotnet restore "./Services/Gym/Gym.API/Gym.API.csproj"
COPY . .
WORKDIR "/src/Services/Gym/Gym.API"
RUN dotnet build "./Gym.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gym.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gym.API.dll"]