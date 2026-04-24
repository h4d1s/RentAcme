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
COPY ["src/Services/Reservation/Reservation.API/Reservation.API.csproj", "src/Services/Reservation/Reservation.API/"]
COPY ["src/Services/Reservation/Reservation.Application/Reservation.Application.csproj", "src/Services/Reservation/Reservation.Application/"]
COPY ["src/BuildingBlocks/Common/Common.csproj", "src/BuildingBlocks/Common/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/GrpcIntegrationHelpers/GrpcIntegrationHelpers.csproj", "src/BuildingBlocks/GrpcIntegrationHelpers/"]
COPY ["src/BuildingBlocks/ConsulIntegrationHelpers/ConsulIntegrationHelpers.csproj", "src/BuildingBlocks/ConsulIntegrationHelpers/"]
COPY ["src/BuildingBlocks/Identity/Identity.csproj", "src/BuildingBlocks/Identity/"]
COPY ["src/Services/Reservation/Reservation.Domain/Reservation.Domain.csproj", "src/Services/Reservation/Reservation.Domain/"]
COPY ["src/Services/Reservation/Reservation.Infrastructure.Persistence/Reservation.Infrastructure.Persistence.csproj", "src/Services/Reservation/Reservation.Infrastructure.Persistence/"]
COPY ["src/BuildingBlocks/Persistence/Persistence.csproj", "src/BuildingBlocks/Persistence/"]
COPY ["src/Services/Reservation/Reservation.Infrastructure/Reservation.Infrastructure.csproj", "src/Services/Reservation/Reservation.Infrastructure/"]
COPY ["src/BuildingBlocks/Diagnostics/Diagnostics.csproj", "src/BuildingBlocks/Diagnostics/"]
COPY ["src/BuildingBlocks/Logging/Logging.csproj", "src/BuildingBlocks/Logging/"]
RUN dotnet restore "./src/Services/Reservation/Reservation.API/Reservation.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Reservation/Reservation.API"
RUN dotnet build "./Reservation.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Reservation.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reservation.API.dll"]