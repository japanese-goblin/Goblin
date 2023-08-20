FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Goblin.WebApp/Goblin.WebApp.csproj", "Goblin.WebApp/"]
COPY ["Goblin.Application.Telegram/Goblin.Application.Telegram.csproj", "Goblin.Application.Telegram/"]
COPY ["Goblin.Application.Core/Goblin.Application.Core.csproj", "Goblin.Application.Core/"]
COPY ["Goblin.DataAccess/Goblin.DataAccess.csproj", "Goblin.DataAccess/"]
COPY ["Goblin.Domain/Goblin.Domain.csproj", "Goblin.Domain/"]
COPY ["Goblin.Narfu/Goblin.Narfu.csproj", "Goblin.Narfu/"]
COPY ["Goblin.OpenWeatherMap/Goblin.OpenWeatherMap.csproj", "Goblin.OpenWeatherMap/"]
COPY ["Goblin.Application.Vk/Goblin.Application.Vk.csproj", "Goblin.Application.Vk/"]
COPY ["Goblin.BackgroundJobs/Goblin.BackgroundJobs.csproj", "Goblin.BackgroundJobs/"]
RUN dotnet restore "Goblin.WebApp/Goblin.WebApp.csproj"
COPY . .
WORKDIR "/src/Goblin.WebApp"
RUN dotnet build "Goblin.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Goblin.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Goblin.WebApp.dll"]