#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Reservation/Reservation.API/Reservation.API.csproj", "src/Services/Reservation/Reservation.API/"]
COPY ["src/Services/Reservation/Reservation.Application/Reservation.Application.csproj", "src/Services/Reservation/Reservation.Application/"]
COPY ["src/Services/Reservation/Reservation.Domain/Reservation.Domain.csproj", "src/Services/Reservation/Reservation.Domain/"]
COPY ["src/Services/Reservation/Reservation.Infrastructure.Persistence/Reservation.Infrastructure.Persistence.csproj", "src/Services/Reservation/Reservation.Infrastructure.Persistence/"]
COPY ["src/Services/Reservation/Reservation.Infrastructure/Reservation.Infrastructure.csproj", "src/Services/Reservation/Reservation.Infrastructure/"]
RUN dotnet restore "./src/Services/Reservation/Reservation.API/Reservation.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Reservation/Reservation.API"
RUN dotnet build "./Reservation.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Reservation.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reservation.API.dll"]