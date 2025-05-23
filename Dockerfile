FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /app

# Copy everything else and build
COPY . ./

RUN dotnet build src/GeoNRage.Server -c Release -o build

RUN dotnet publish src/GeoNRage.Server -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "GeoNRage.Server.dll"]
