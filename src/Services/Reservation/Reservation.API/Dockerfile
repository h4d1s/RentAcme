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
COPY ["Services/Reservation/Reservation.API/Reservation.API.csproj", "Services/Reservation/Reservation.API/"]
COPY ["Services/Reservation/Reservation.Application/Reservation.Application.csproj", "Services/Reservation/Reservation.Application/"]
COPY ["BuildingBlocks/Common/Common.csproj", "BuildingBlocks/Common/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/GrpcIntegrationHelpers/GrpcIntegrationHelpers.csproj", "BuildingBlocks/GrpcIntegrationHelpers/"]
COPY ["BuildingBlocks/ConsulIntegrationHelpers/ConsulIntegrationHelpers.csproj", "BuildingBlocks/ConsulIntegrationHelpers/"]
COPY ["BuildingBlocks/Identity/Identity.csproj", "BuildingBlocks/Identity/"]
COPY ["Services/Reservation/Reservation.Domain/Reservation.Domain.csproj", "Services/Reservation/Reservation.Domain/"]
COPY ["Services/Reservation/Reservation.Infrastructure.Persistence/Reservation.Infrastructure.Persistence.csproj", "Services/Reservation/Reservation.Infrastructure.Persistence/"]
COPY ["BuildingBlocks/Persistence/Persistence.csproj", "BuildingBlocks/Persistence/"]
COPY ["Services/Reservation/Reservation.Infrastructure/Reservation.Infrastructure.csproj", "Services/Reservation/Reservation.Infrastructure/"]
COPY ["BuildingBlocks/Diagnostics/Diagnostics.csproj", "BuildingBlocks/Diagnostics/"]
COPY ["BuildingBlocks/Logging/Logging.csproj", "BuildingBlocks/Logging/"]
RUN dotnet restore "Services/Reservation/Reservation.API/Reservation.API.csproj"
COPY . .
WORKDIR "/src/Services/Reservation/Reservation.API"
RUN dotnet build "Reservation.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Reservation.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reservation.API.dll"]