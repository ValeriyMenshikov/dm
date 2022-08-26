# Установка и запуск для разработчиков

1. Нужно установить на компьютер Docker for Desktop последней версии. Для пользователей Windows настоятельно рекомендуется переключиться с Hyper-V на WSL2 в качестве движка виртуализации. В разработке мы пользуемся только вторым, поэтому не гарантируем корректную работу на Hyper-V.
2. Для локальной разработки бэка нужно установить на компьютер dotnet5.0
3Для локальной разработки фронта нужно установить на компьютер Node.js последней мажорной версии
3. Запустить в директории репозитория `./DM` команду `docker compose up -d`, которая скачает и запустит все необходимые для окружения DM приложения
4. Запустить в директории репозитория `./DM/Services/DM.Services.DataAccess` команду `dotnet ef database update`, которая проинициализирует базу данных в готовом для работы состоянии. Если не получается, попробуйте сперва вызвать команду `dotnet tool install --global dotnet-ef`.

## Запуск бэка

Бэк состоит из одного API-приложения и трех консюмеров. 

Для их запуска пользователям Visual Studio нужно выбрать проекты `DM.Services.Notifications.Consumer`, `DM.Services.Search.Consumer`, `DM.Services.Mail.Sender.Consumer` и `DM.Web.API`, выбрать для каждого из них запуск в self-hosted режиме (не в IIS-Express, та же причина, что и для выбора WSL2), а затем сконфигурировать их совместный запуск.

Пользователям Rider можно просто запускать эти сервисы друг за другом.

## Запуск фронта

```bash
cd ./DM/Web/DM.Web.Modern
npm i -g yarn
yarn & yarn run serve
```

# Установка и запуск для Валеры

1. Нужно установить на компьютер Docker for Desktop последней версии. Для пользователей Windows настоятельно рекомендуется переключиться с Hyper-V на WSL2 в качестве движка виртуализации. В разработке мы пользуемся только вторым, поэтому не гарантируем корректную работу на Hyper-V
2. Запустить в директории репозитория `./DM` команду `docker compose up -d`, которая скачает и запустит все необходимые для окружения DM приложения и сам DM
3. Открыть в браузере `http://localhost:9001`, войти под `minio:miniokey`
4. Перейти на вкладку "Buckets", создать новый бакет под именем `dm-uploads`, в настройках бакета после создания изменить уровень доступа с `Private` на `Public`
5. Swagger API доступен по `http://localhost:5051`
6. Фронтенд доступен по `http://localhost:5050`

## Создание пользователя