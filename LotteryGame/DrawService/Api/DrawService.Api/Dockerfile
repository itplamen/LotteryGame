# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DrawService/Api/DrawService.Api/DrawService.Api.csproj", "DrawService/Api/DrawService.Api/"]
COPY ["DrawService/Core/DrawService.Core/DrawService.Core.csproj", "DrawService/Core/DrawService.Core/"]
COPY ["DrawService/Data/DrawService.Data.Models/DrawService.Data.Models.csproj", "DrawService/Data/DrawService.Data.Models/"]
COPY ["DrawService/Data/DrawService.Data/DrawService.Data.csproj", "DrawService/Data/DrawService.Data/"]
COPY ["DrawService/Core/DrawService.Core.Models/DrawService.Core.Models.csproj", "DrawService/Core/DrawService.Core.Models/"]
COPY ["Common/LotteryGame.Common.Models/LotteryGame.Common.Models.csproj", "Common/LotteryGame.Common.Models/"]
RUN dotnet restore "./DrawService/Api/DrawService.Api/DrawService.Api.csproj"
COPY . .
WORKDIR "/src/DrawService/Api/DrawService.Api"
RUN dotnet build "./DrawService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DrawService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DrawService.Api.dll"]