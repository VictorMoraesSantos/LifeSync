# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Nutrition/Nutrition.API/Nutrition.API.csproj", "Services/Nutrition/Nutrition.API/"]
COPY ["Core/Core.API/Core.API.csproj", "Core/Core.API/"]
COPY ["Services/Nutrition/Nutrition.Infrastructure/Nutrition.Infrastructure.csproj", "Services/Nutrition/Nutrition.Infrastructure/"]
COPY ["Core/Core.Infrastructure/Core.Infrastructure.csproj", "Core/Core.Infrastructure/"]
COPY ["Core/Core.Domain/Core.Domain.csproj", "Core/Core.Domain/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/Nutrition/Nutrition.Application/Nutrition.Application.csproj", "Services/Nutrition/Nutrition.Application/"]
COPY ["Core/Core.Application/Core.Application.csproj", "Core/Core.Application/"]
COPY ["Services/Nutrition/Nutrition.Domain/Nutrition.Domain.csproj", "Services/Nutrition/Nutrition.Domain/"]
RUN dotnet restore "./Services/Nutrition/Nutrition.API/Nutrition.API.csproj"
COPY . .
WORKDIR "/src/Services/Nutrition/Nutrition.API"
RUN dotnet build "./Nutrition.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nutrition.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nutrition.API.dll"]