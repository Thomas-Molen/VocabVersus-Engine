FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Install nuGet packages
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/build .
ENTRYPOINT ["dotnet", "vocabversus-engine.dll"]