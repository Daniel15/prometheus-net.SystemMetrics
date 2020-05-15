#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Prometheus.SystemMetrics.Example/Prometheus.SystemMetrics.Example.csproj", "src/Prometheus.SystemMetrics.Example/"]
RUN dotnet restore "src/Prometheus.SystemMetrics.Example/Prometheus.SystemMetrics.Example.csproj"
COPY . .
WORKDIR "/src/src/Prometheus.SystemMetrics.Example"
RUN dotnet build "Prometheus.SystemMetrics.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Prometheus.SystemMetrics.Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Prometheus.SystemMetrics.Example.dll"]