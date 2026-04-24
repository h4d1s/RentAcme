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
COPY ["Services/User/User.API/User.API.csproj", "Services/User/User.API/"]
COPY ["BuildingBlocks/Common/Common.csproj", "BuildingBlocks/Common/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["Services/User/User.Application/User.Application.csproj", "Services/User/User.Application/"]
COPY ["BuildingBlocks/Identity/Identity.csproj", "BuildingBlocks/Identity/"]
COPY ["Services/User/User.Domain/User.Domain.csproj", "Services/User/User.Domain/"]
COPY ["Services/User/User.Infrastructure.Persistence/User.Infrastructure.Persistence.csproj", "Services/User/User.Infrastructure.Persistence/"]
COPY ["BuildingBlocks/Persistence/Persistence.csproj", "BuildingBlocks/Persistence/"]
COPY ["Services/User/User.Infrastructure/User.Infrastructure.csproj", "Services/User/User.Infrastructure/"]
COPY ["BuildingBlocks/ConsulIntegrationHelpers/ConsulIntegrationHelpers.csproj", "BuildingBlocks/ConsulIntegrationHelpers/"]
COPY ["BuildingBlocks/GrpcIntegrationHelpers/GrpcIntegrationHelpers.csproj", "BuildingBlocks/GrpcIntegrationHelpers/"]
COPY ["BuildingBlocks/Diagnostics/Diagnostics.csproj", "BuildingBlocks/Diagnostics/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
RUN dotnet restore "./Services/User/User.API/User.API.csproj"
COPY . .
WORKDIR "/src/Services/User/User.API"
RUN dotnet build "./User.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./User.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.API.dll"]