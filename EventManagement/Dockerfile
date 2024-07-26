# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Build and publish image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EventManagement/EventManagement.csproj", "EventManagement/"]
RUN dotnet restore "EventManagement/EventManagement.csproj"
COPY . .
WORKDIR "/src/EventManagement"
RUN dotnet publish "EventManagement.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EventManagement.dll"]