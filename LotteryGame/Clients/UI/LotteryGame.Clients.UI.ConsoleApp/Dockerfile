# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Clients/UI/LotteryGame.Clients.UI.ConsoleApp/LotteryGame.Clients.UI.ConsoleApp.csproj", "Clients/UI/LotteryGame.Clients.UI.ConsoleApp/"]
COPY ["Clients/Core/LotteryGame.Clients.Core.Services/LotteryGame.Clients.Core.Services.csproj", "Clients/Core/LotteryGame.Clients.Core.Services/"]
COPY ["Common/LotteryGame.Common.Models/LotteryGame.Common.Models.csproj", "Common/LotteryGame.Common.Models/"]
COPY ["Orchestrators/Core/LotteryGame.Orchestrators.Models/LotteryGame.Orchestrators.Models.csproj", "Orchestrators/Core/LotteryGame.Orchestrators.Models/"]
COPY ["Clients/Core/LotteryGame.Clients.Core.Services.Models/LotteryGame.Clients.Core.Services.Models.csproj", "Clients/Core/LotteryGame.Clients.Core.Services.Models/"]
COPY ["Clients/Core/LotteryGame.Clients.Core.Wrapper/LotteryGame.Clients.Core.Wrapper.csproj", "Clients/Core/LotteryGame.Clients.Core.Wrapper/"]
RUN dotnet restore "./Clients/UI/LotteryGame.Clients.UI.ConsoleApp/LotteryGame.Clients.UI.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/Clients/UI/LotteryGame.Clients.UI.ConsoleApp"
RUN dotnet build "./LotteryGame.Clients.UI.ConsoleApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LotteryGame.Clients.UI.ConsoleApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LotteryGame.Clients.UI.ConsoleApp.dll"]