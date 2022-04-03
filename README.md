# Goblin - бот для ВКонтакте и Telegram

Сайт - https://goblin-safu.herokuapp.com (или https://japanese-goblin.site)

Группа ВК - https://vk.com/club146048760

Бот в Telegram - https://t.me/japanese_goblin_bot

Первая версия PHP - 29.04.2017

Первая версия C# - 08.02.2018

# Запуск проекта
## 1. С докером
```shell
cd Goblin
docker compose up
```

Проект будет запущен на http://localhost:5177.

При необходимости поменять конфиг в .env файле
## 2. Без докера
Для запуска необходимо поднять Postgres
```shell
cd Goblin
dotnet restore
dotnet run --project src/Goblin.WebApp/Goblin.WebApp.csproj
```