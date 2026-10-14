# Task Service

Простое API сервиса задач на .NET 8, упакованное в Docker-контейнер.

Учебная работа: **API → Dockerfile → docker build → docker run -p → запрос к API**.

## Тема проекта

Сервис задач. API отдаёт список задач и позволяет проверить, что сервис работает.

## Endpoints

| Метод | Путь      | Описание                                      |
|-------|-----------|-----------------------------------------------|
| GET   | `/tasks`  | Возвращает список задач в формате JSON        |
| GET   | `/health` | Проверка работоспособности, ответ `{"status":"ok"}` |

Пример ответа `GET /tasks`:

```json
[
  { "id": 1, "title": "Написать Dockerfile", "done": false },
  { "id": 2, "title": "Собрать образ", "done": false }
]
```

## Структура проекта

```
TaskService/
├── Program.cs          # исходный код API
├── TaskService.csproj  # файл проекта .NET
├── Dockerfile          # инструкция сборки образа
├── .dockerignore       # что не копировать в образ
├── README.md           # этот файл
├── docker-ps.txt       # вывод docker ps
├── api-response.txt    # ответ API
└── docker-logs.txt     # вывод docker logs
```

## Требования

- .NET 8 SDK (только для запуска без Docker)
- Docker Desktop или Docker Engine

## Локальный запуск (без Docker)

```bash
dotnet run --urls http://localhost:5050
```

Проверка в браузере или через curl:

```bash
curl http://localhost:5050/tasks
curl http://localhost:5050/health
```

Остановить приложение: `Ctrl+C`.

> На macOS порт 5000 занят системой (AirPlay), поэтому используется 5050.

## Сборка Docker-образа

```bash
docker build -t task-service .
```

## Запуск контейнера

```bash
docker run -d --name task-service -p 8080:8080 task-service
```

- `-d` — запуск в фоне
- `--name task-service` — имя контейнера
- `-p 8080:8080` — проброс порта (порт хоста : порт контейнера)

## Проверка

Список запущенных контейнеров:

```bash
docker ps
```

Запрос к API:

```bash
curl http://localhost:8080/tasks
curl http://localhost:8080/health
```

Или открыть в браузере: <http://localhost:8080/tasks>

Логи контейнера:

```bash
docker logs task-service
```

## Остановка и удаление контейнера

```bash
docker stop task-service
docker rm task-service
```

## Что делает Dockerfile

| Инструкция | Назначение |
|------------|------------|
| `FROM ...sdk:8.0 AS build` | Образ с .NET SDK для сборки приложения |
| `WORKDIR /src` | Рабочая папка внутри контейнера |
| `COPY TaskService.csproj .` + `RUN dotnet restore` | Скачивание зависимостей (слой кэшируется) |
| `COPY . .` | Копирование исходного кода |
| `RUN dotnet publish -c Release -o /app` | Сборка приложения в папку `/app` |
| `FROM ...aspnet:8.0` | Лёгкий runtime-образ для запуска |
| `COPY --from=build /app .` | Перенос готовой сборки из первого этапа |
| `ENV ASPNETCORE_URLS=http://+:8080` | Приложение слушает порт 8080 на всех интерфейсах |
| `EXPOSE 8080` | Документирует используемый порт |
| `ENTRYPOINT ["dotnet", "TaskService.dll"]` | Команда запуска приложения |

## Результаты проверки

- [docker-ps.txt](docker-ps.txt) — работающий контейнер и проброшенный порт
- [api-response.txt](api-response.txt) — ответ API на `GET /tasks`
- [docker-logs.txt](docker-logs.txt) — логи контейнера
