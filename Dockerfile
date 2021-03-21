FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./
RUN dotnet tool restore

# Access to the global tool: https://stackoverflow.com/a/51984439
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet build src/GeoNRage.Server -c Release -o build

RUN dotnet publish src/GeoNRage.Server -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "GeoNRage.Server.dll"]
