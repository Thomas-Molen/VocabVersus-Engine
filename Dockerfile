# Create environment for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App
# Copy everything from the current directory into the working directory
COPY . ./
# Install nuGet packages
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o build

# Create environment for running the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
# Move the built app to working directory
COPY --from=build-env /App/build .
# Expose port 80 for HTTP API and SignalR websocket
EXPOSE 80
# Run dotnet tool to start application
ENTRYPOINT ["dotnet", "vocabversus-engine.dll"]