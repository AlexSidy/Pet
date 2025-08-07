# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8090
EXPOSE 8091

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ScanPerson.Auth.Api/ScanPerson.Auth.Api.csproj", "ScanPerson.Auth.Api/"]
COPY ["ScanPerson.Models/ScanPerson.Models.csproj", "ScanPerson.Models/"]
COPY ["ScanPerson.Models/ScanPerson.Common.csproj", "ScanPerson.Common/"]
RUN dotnet restore "ScanPerson.Auth.Api/ScanPerson.Auth.Api.csproj"
COPY . .
WORKDIR "/src/ScanPerson.Auth.Api"
RUN dotnet build "ScanPerson.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ScanPerson.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScanPerson.Auth.Api.dll"]