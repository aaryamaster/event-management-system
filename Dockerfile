# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY EventManagement/EventManagement.csproj EventManagement/
COPY EventManagement.Tests/EventManagement.Tests.csproj EventManagement.Tests/

# Restore dependencies and build the project
RUN dotnet restore EventManagement/EventManagement.csproj
COPY . .
RUN dotnet build EventManagement/EventManagement.csproj --configuration Release --no-restore

# Publish the application
RUN dotnet publish EventManagement/EventManagement.csproj --configuration Release --output /app/publish --no-build

# Use the ASP.NET image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Install Cloud SQL Auth Proxy
RUN wget https://dl.google.com/cloudsql/cloud_sql_proxy.darwin.arm64 -O /usr/local/bin/cloud_sql_proxy && \
    chmod +x /usr/local/bin/cloud_sql_proxy

# Expose ports
EXPOSE 8080
EXPOSE 443

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Start Cloud SQL Auth Proxy and your application
CMD ["/bin/sh", "-c", "/usr/local/bin/cloud_sql_proxy -dir=/cloudsql -instances=event-management-system-430621:us-central1:eventmanagementsystem & dotnet EventManagement.dll"]
