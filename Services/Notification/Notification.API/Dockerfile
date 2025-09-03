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
COPY ["Services/EmailSender/EmailSender.API/EmailSender.API.csproj", "Services/EmailSender/EmailSender.API/"]
COPY ["Services/EmailSender/EmailSender.Infrastructure/EmailSender.Infrastructure.csproj", "Services/EmailSender/EmailSender.Infrastructure/"]
COPY ["Services/EmailSender/EmailSender.Application/EmailSender.Application.csproj", "Services/EmailSender/EmailSender.Application/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/EmailSender/EmailSender.Domain/EmailSender.Domain.csproj", "Services/EmailSender/EmailSender.Domain/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
COPY ["Core/Core.Domain/Core.Domain.csproj", "Core/Core.Domain/"]
RUN dotnet restore "./Services/EmailSender/EmailSender.API/EmailSender.API.csproj"
COPY . .
WORKDIR "/src/Services/EmailSender/EmailSender.API"
RUN dotnet build "./EmailSender.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EmailSender.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailSender.API.dll"]