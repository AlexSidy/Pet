# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ScanPerson.WebApi/ScanPerson.WebApi.csproj", "ScanPerson.WebApi/"]
COPY ["ScanPerson.Models/ScanPerson.Models.csproj", "ScanPerson.Models/"]
COPY ["ScanPerson.Models/ScanPerson.Common.csproj", "ScanPerson.Common/"]
COPY ["Scanperson.DAL/Scanperson.DAL.csproj", "Scanperson.DAL/"]
COPY ["ScanPerson.BusinessLogic/ScanPerson.BusinessLogic.csproj", "ScanPerson.BusinessLogic/"]
RUN dotnet restore "ScanPerson.WebApi/ScanPerson.WebApi.csproj"
COPY . .

WORKDIR "/src/ScanPerson.WebApi"
RUN dotnet build "ScanPerson.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ScanPerson.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "ScanPerson.WebApi.dll"]
