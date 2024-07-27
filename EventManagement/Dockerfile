# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

# Build and test image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["EventManagement/EventManagement.csproj", "EventManagement/"]
COPY ["EventManagement.Tests/EventManagement.Tests.csproj", "EventManagement.Tests/"]
RUN dotnet restore "EventManagement/EventManagement.csproj"

# Copy all files and build the main project
COPY . .
WORKDIR "/src/EventManagement"
RUN dotnet build "EventManagement.csproj" -c Release -o /app/build

# Run tests
WORKDIR "/src/EventManagement.Tests"
RUN dotnet test --no-restore --verbosity normal

# Publish the application
FROM build AS publish
RUN dotnet publish "EventManagement.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the environment variable to tell ASP.NET Core to bind to port 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EventManagement.dll"]
