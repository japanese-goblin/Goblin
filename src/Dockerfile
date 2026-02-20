FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
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
RUN dotnet build "./Goblin.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Goblin.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
RUN apk add --no-cache \
    icu-libs \
    krb5-libs \
    libgdiplus \
    zlib \
    locales \
    locales-all \
    && rm -rf /var/cache/apk/*

ENV TZ="Europe/Moscow"
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LC_ALL=ru_RU.UTF-8
ENV LANG=ru_RU.UTF-8

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Goblin.WebApp.dll"]
