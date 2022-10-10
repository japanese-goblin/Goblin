FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY . .
RUN dotnet restore
WORKDIR "Goblin.WebApp"
RUN dotnet build "Goblin.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Goblin.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Goblin.WebApp.dll"]