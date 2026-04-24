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
COPY ["Services/Inventory/Inventory.API/Inventory.API.csproj", "Services/Inventory/Inventory.API/"]
COPY ["Services/Inventory/Inventory.Application/Inventory.Application.csproj", "Services/Inventory/Inventory.Application/"]
COPY ["BuildingBlocks/Common/Common.csproj", "BuildingBlocks/Common/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["Services/Inventory/Inventory.Domain/Inventory.Domain.csproj", "Services/Inventory/Inventory.Domain/"]
COPY ["Services/Inventory/Inventory.Infrastructure.Persistence/Inventory.Infrastructure.Persistence.csproj", "Services/Inventory/Inventory.Infrastructure.Persistence/"]
COPY ["BuildingBlocks/Persistence/Persistence.csproj", "BuildingBlocks/Persistence/"]
COPY ["Services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj", "Services/Inventory/Inventory.Infrastructure/"]
COPY ["BuildingBlocks/ConsulIntegrationHelpers/ConsulIntegrationHelpers.csproj", "BuildingBlocks/ConsulIntegrationHelpers/"]
COPY ["BuildingBlocks/Diagnostics/Diagnostics.csproj", "BuildingBlocks/Diagnostics/"]
COPY ["BuildingBlocks/GrpcIntegrationHelpers/GrpcIntegrationHelpers.csproj", "BuildingBlocks/GrpcIntegrationHelpers/"]
COPY ["BuildingBlocks/Identity/Identity.csproj", "BuildingBlocks/Identity/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
RUN dotnet restore "Services/Inventory/Inventory.API/Inventory.API.csproj"
COPY . .
WORKDIR "/src/Services/Inventory/Inventory.API"
RUN dotnet build "Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]