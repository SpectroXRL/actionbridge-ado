# Restore packages and compile the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only project files first (for layer caching)
# This allows Docker to cache the restore step if only code changes, not dependencies
COPY ["src/ActionBridge-Ado.Api/ActionBridge-Ado.Api.csproj", "src/ActionBridge-Ado.Api/"]
RUN dotnet restore "src/ActionBridge-Ado.Api/ActionBridge-Ado.Api.csproj"

# Now copy everything else
COPY . .

# Build in Release mode
WORKDIR "/src/src/ActionBridge-Ado.Api"
RUN dotnet build "ActionBridge-Ado.Api.csproj" -c Release -o /app/build

# Create the deployment-ready artifacts
FROM build AS publish
RUN dotnet publish "ActionBridge-Ado.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the smallest possible production image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Security: Run as non-root user
USER $APP_UID

# Copy only the published output from the publish stage
COPY --from=publish /app/publish .

# Expose the port your app listens on (default for ASP.NET Core)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "ActionBridge-Ado.Api.dll"]