# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["*.csproj", "./"]
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final lightweight & secure runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS final
WORKDIR /app

# Copy the published files from the 'publish' stage
COPY --from=publish /app/publish .

# Configure the app to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Security: Switch to the built-in non-root user provided by Microsoft's .NET images
USER $APP_UID

# Start the application
ENTRYPOINT ["dotnet", "platform-demo.dll"]