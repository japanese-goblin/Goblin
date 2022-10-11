# Goblin - бот для ВКонтакте и Telegram

Сайт - https://goblin-safu.herokuapp.com (или https://japanese-goblin.site)

Группа ВК - https://vk.com/club146048760

Бот в Telegram - https://t.me/japanese_goblin_bot

Первая версия PHP - 29.04.2017

Первая версия C# - 08.02.2018

# Запуск проекта
## С докером
```shell
cd Goblin
docker compose up
```

Проект будет запущен на http://localhost:5177.

При необходимости поменять конфиг в .env файле

## Без докера
Для запуска необходимо поднять Postgres
```shell
cd Goblin
dotnet restore
dotnet run --project src/Goblin.WebApp/Goblin.WebApp.csproj
```

# Деплой
1. Создать volume для сертификатов:
```shell
docker volume create goblin-certs
```
2. Поместить туда `public.cert` и `private.cert`
3. Создать .env файл:
```shell
touch goblin.env
```
4. Установить свои значения для конфига:
```env
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=goblin;User ID=postgres;Password=postgres;Pooling=true;
OWM__AccessToken=DEV_ACCESS_TOKEN
Telegram__AccessToken=DEV_ACCESS_TOKEN
Telegram_SecretKey=DEV_SECRET_KEY
Vk__AccessToken=DEV_ACCESS_TOKEN
Vk__ConfirmationCode=DEV_CONFIRM_CODE
Vk__SecretKey=DEV_SECRET_KEY
Github__ClientId=DEV_CLIENT_ID
Github__ClientSecret=DEV_CLIENT_SECRET
ASPNETCORE_ENVIRONMENT=Production
DOTNET_ENVIRONMENT=Production
TZ=Europe/Moscow
```
5. Запустить проект:
```shell
docker run -d --restart=always -p 80:80 -p 443:443 -v goblin-certs:/app/certs --name goblin-docker --env-file goblin.env --network goblin_goblin_network ghcr.io/japanese-goblin/goblin:main
```
