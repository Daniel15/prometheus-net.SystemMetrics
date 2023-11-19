#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Prometheus.SystemMetrics.Example/Prometheus.SystemMetrics.Example.csproj", "src/Prometheus.SystemMetrics.Example/"]
RUN dotnet restore "src/Prometheus.SystemMetrics.Example/Prometheus.SystemMetrics.Example.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Prometheus.SystemMetrics.Example.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApplication4.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Prometheus.SystemMetrics.Example.dll"]