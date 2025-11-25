# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/EquipmentManagement.Domain/EquipmentManagement.Domain.csproj", "src/EquipmentManagement.Domain/"]
COPY ["src/EquipmentManagement.Application/EquipmentManagement.Application.csproj", "src/EquipmentManagement.Application/"]
COPY ["src/EquipmentManagement.Infrastructure/EquipmentManagement.Infrastructure.csproj", "src/EquipmentManagement.Infrastructure/"]
COPY ["src/EquipmentManagement.WebAPI/EquipmentManagement.WebAPI.csproj", "src/EquipmentManagement.WebAPI/"]

# Restore dependencies
RUN dotnet restore "src/EquipmentManagement.WebAPI/EquipmentManagement.WebAPI.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/EquipmentManagement.WebAPI"
RUN dotnet build "EquipmentManagement.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "EquipmentManagement.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "EquipmentManagement.WebAPI.dll"]
