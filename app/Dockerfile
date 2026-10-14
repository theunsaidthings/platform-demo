# ==========================================
# Stage 1: Build & Publish
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first to take advantage of layer caching for NuGet packages
COPY *.csproj ./
RUN dotnet restore

# Copy all source code and publish release artifacts
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Stage 2: Hardened Runtime Container
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy compiled application binaries
COPY --from=build /app/publish .

# Copy and configure entrypoint script permissions
COPY entrypoint.sh .
USER root
RUN chmod +x ./entrypoint.sh

# Temporarily use root to install curl and set permissions
USER root
RUN apt-get update && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/* \
    && chmod +x ./entrypoint.sh

# Switch to built-in non-root user for security compliance
USER $APP_UID

# Network binding: listen on port 5000
ENV ASPNETCORE_URLS=http://+:9001
EXPOSE 9001

# Polls the readiness endpoint every 10 seconds. Fails after 3 timeouts.
HEALTHCHECK --interval=10s --timeout=3s --start-period=5s --retries=3 CMD curl -f http://localhost:9001/health/readiness || exit 1

# Execute pre-flight checks and start application
ENTRYPOINT ["./entrypoint.sh"]
CMD ["dotnet", "platform-demo.dll"]