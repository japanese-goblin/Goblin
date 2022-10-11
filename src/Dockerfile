FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80;https://+:80
ENV Kestrel__Endpoints__HttpsFromPem__Url=https://0.0.0.0:443
ENV Kestrel__Endpoints__HttpsFromPem__Certificate__KeyPath=certs/private.cert
ENV Kestrel__Endpoints__HttpsFromPem__Certificate__Path=certs/public.cert

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY . .
RUN dotnet restore
WORKDIR "Goblin.WebApp"
RUN dotnet build "Goblin.WebApp.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Goblin.WebApp.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Goblin.WebApp.dll"]
