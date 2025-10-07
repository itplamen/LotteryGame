# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DrawService/Workers/DrawService.Workers/DrawService.Workers.csproj", "DrawService/Workers/DrawService.Workers/"]
COPY ["DrawService/Core/DrawService.Core/DrawService.Core.csproj", "DrawService/Core/DrawService.Core/"]
COPY ["DrawService/Data/DrawService.Data.Models/DrawService.Data.Models.csproj", "DrawService/Data/DrawService.Data.Models/"]
COPY ["DrawService/Data/DrawService.Data/DrawService.Data.csproj", "DrawService/Data/DrawService.Data/"]
COPY ["DrawService/Core/DrawService.Core.Models/DrawService.Core.Models.csproj", "DrawService/Core/DrawService.Core.Models/"]
COPY ["Common/LotteryGame.Common.Models/LotteryGame.Common.Models.csproj", "Common/LotteryGame.Common.Models/"]
COPY ["DrawService/Workers/DrawService.Workers.Infrastructure/DrawService.Workers.Infrastructure.csproj", "DrawService/Workers/DrawService.Workers.Infrastructure/"]
RUN dotnet restore "./DrawService/Workers/DrawService.Workers/DrawService.Workers.csproj"
COPY . .
WORKDIR "/src/DrawService/Workers/DrawService.Workers"
RUN dotnet build "./DrawService.Workers.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DrawService.Workers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DrawService.Workers.dll"]