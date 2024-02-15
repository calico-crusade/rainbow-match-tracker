FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet publish "./src/RainbowMatchTracker.Api/RainbowMatchTracker.Api.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "RainbowMatchTracker.Api.dll"]

# https://docs.docker.com/engine/examples/dotnetcore/