# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Orchestrators/Api/LotteryGame.Orchestrators.Api/LotteryGame.Orchestrators.Api.csproj", "Orchestrators/Api/LotteryGame.Orchestrators.Api/"]
COPY ["Common/LotteryGame.Common.Models/LotteryGame.Common.Models.csproj", "Common/LotteryGame.Common.Models/"]
COPY ["Orchestrators/Core/LotteryGame.Orchestrators/LotteryGame.Orchestrators.csproj", "Orchestrators/Core/LotteryGame.Orchestrators/"]
COPY ["Orchestrators/Core/LotteryGame.Orchestrators.Gateways/LotteryGame.Orchestrators.Gateways.csproj", "Orchestrators/Core/LotteryGame.Orchestrators.Gateways/"]
COPY ["Orchestrators/Core/LotteryGame.Orchestrators.Cache/LotteryGame.Orchestrators.Cache.csproj", "Orchestrators/Core/LotteryGame.Orchestrators.Cache/"]
COPY ["Orchestrators/Core/LotteryGame.Orchestrators.Models/LotteryGame.Orchestrators.Models.csproj", "Orchestrators/Core/LotteryGame.Orchestrators.Models/"]
RUN dotnet restore "./Orchestrators/Api/LotteryGame.Orchestrators.Api/LotteryGame.Orchestrators.Api.csproj"
COPY . .
WORKDIR "/src/Orchestrators/Api/LotteryGame.Orchestrators.Api"
RUN dotnet build "./LotteryGame.Orchestrators.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LotteryGame.Orchestrators.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LotteryGame.Orchestrators.Api.dll"]