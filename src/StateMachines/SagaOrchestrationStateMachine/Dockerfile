#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/StateMachines/SagaOrchestrationStateMachine/SagaOrchestrationStateMachine.csproj", "src/StateMachines/SagaOrchestrationStateMachine/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/Persistence/Persistence.csproj", "src/BuildingBlocks/Persistence/"]
RUN dotnet restore "./src/StateMachines/SagaOrchestrationStateMachine/SagaOrchestrationStateMachine.csproj"
COPY . .
WORKDIR "/src/src/StateMachines/SagaOrchestrationStateMachine"
RUN dotnet build "./SagaOrchestrationStateMachine.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SagaOrchestrationStateMachine.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SagaOrchestrationStateMachine.dll"]