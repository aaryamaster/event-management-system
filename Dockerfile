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
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EventManagement.dll"]
