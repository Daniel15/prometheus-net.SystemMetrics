#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Prometheus.SystemMetrics.Example.AOT/Prometheus.SystemMetrics.Example.AOT.csproj", "src/Prometheus.SystemMetrics.Example.AOT/"]
RUN dotnet restore "./src/Prometheus.SystemMetrics.Example.AOT/./Prometheus.SystemMetrics.Example.AOT.csproj"
COPY . .
WORKDIR "/src/src/Prometheus.SystemMetrics.Example.AOT"
RUN dotnet build "./Prometheus.SystemMetrics.Example.AOT.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Prometheus.SystemMetrics.Example.AOT.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["./Prometheus.SystemMetrics.Example.AOT"]