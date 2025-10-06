# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WagerService/Api/WagerService.Api/WagerService.Api.csproj", "WagerService/Api/WagerService.Api/"]
COPY ["WagerService/Core/WagerService.Core/WagerService.Core.csproj", "WagerService/Core/WagerService.Core/"]
COPY ["Common/LotteryGame.Common.Models/LotteryGame.Common.Models.csproj", "Common/LotteryGame.Common.Models/"]
COPY ["WagerService/Data/WagerService.Data/WagerService.Data.csproj", "WagerService/Data/WagerService.Data/"]
COPY ["WagerService/Data/WagerService.Data.Models/WagerService.Data.Models.csproj", "WagerService/Data/WagerService.Data.Models/"]
COPY ["WagerService/Core/WagerService.Core.Models/WagerService.Core.Models.csproj", "WagerService/Core/WagerService.Core.Models/"]
RUN dotnet restore "./WagerService/Api/WagerService.Api/WagerService.Api.csproj"
COPY . .
WORKDIR "/src/WagerService/Api/WagerService.Api"
RUN dotnet build "./WagerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WagerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WagerService.Api.dll"]