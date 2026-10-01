# UI Autotests

Автотесты UI-приложения на **C#**. Стек: **MSTest** + **Playwright** + **Allure Report**.

Решение из нескольких модулей: каждый функциональный блок приложения вынесен
в отдельный проект, общая инфраструктура — в `BaseProject`.

Покрытые сценарии: дефекты, оперативный журнал, осмотры оборудования,
приём/передача смены, управление сменами, оформление нарядов.

---

## Стек

| Компонент | Версия | Назначение |
|---|---|---|
| .NET | 10.0 | Целевой фреймворк |
| MSTest | 4.4 | Тестовый раннер |
| Playwright | 1.62 | Управление браузером (Chromium) и HTTP-запросы |
| Allure.Net.Commons | 2.15 | Отчётность |

---

## Структура решения

```
UITests/
├── BaseProject/                  # Общая инфраструктура всех модулей
│   ├── Base/
│   │   ├── Config.cs             #   Конфигурация окружения
│   │   ├── Crew.cs               #   Участники бригады для нарядов
│   │   ├── PlayInit.cs           #   Браузер, контексты, страница
│   │   └── BaseClass.cs          #   Общие поля данных теста
│   ├── AllureHelpers/            #   Жизненный цикл Allure, атрибуты, аспекты
│   ├── Extensions/
│   │   ├── APIExtensions.cs      #   Запросы данных через HTTP API
│   │   └── PageExtensions.cs     #   Помощники для страницы (прокрутка деревьев)
│   ├── Helpers/                  #   Сборка JSON-полезной нагрузки
│   │   ├── JsonBuilder.cs
│   │   └── LuaScriptJsonBuilder.cs
│   └── Source/Lua/               #   Скрипты выборки тестовых данных
│
├── DefectsTests/                 # Дефекты
│   ├── BaseDefects/              #   Инициализация модуля
│   ├── Map/                      #   Page Objects
│   └── Tests/CreateDefect.cs
│
├── DispJournalTests/             # Оперативный журнал
│   ├── BaseDisp/
│   ├── Extensions/
│   ├── Map/
│   ├── Source/Lua/
│   └── Tests/CreateRecord.cs
│
├── EquipmentInspectionsTests/    # Осмотры оборудования
│   ├── Base/
│   ├── Map/
│   └── Tests/CreateStatement.cs
│
├── ShiftHandoverTests/           # Приём и передача смены
│   ├── BaseShiftHandover/
│   ├── Extensions/
│   ├── Map/
│   ├── Source/lua/
│   └── Tests/
│
├── ShiftManagerTests/            # Управление сменами
│   ├── BaseShiftManager/
│   ├── Extensions/
│   ├── Map/
│   └── Tests/ForceShiftTransfer.cs
│
└── WorkOrdersTests/              # Наряды-допуски и распоряжения
    └── WorkOrdersTests/
        ├── BaseWorkOrders/       #   Инициализация модуля
        ├── Extensions/           #   Помощники (выбор в дереве, заполнение форм)
        ├── Map/                  #   Page Objects
        ├── Source/Lua/           #   Скрипты выборки данных
        └── Tests/
            ├── CreateOrder.cs
            └── CreateRecordEU.cs
```

### Архитектурные принципы

**Изоляция модулей.** Каждый функциональный блок — отдельный проект со своей
папкой `Base` (инициализация), `Map` (Page Objects), `Extensions` (помощники)
и `Tests` (сценарии). Модуль можно запускать и дорабатывать независимо.

**Page Object Model.** Каждая сущность интерфейса описана отдельным классом
в `Map`. Локаторы инкапсулированы в свойства, действия вынесены в методы.
Тест читается как сценарий, а не как набор селекторов.

**Подготовка данных через API.** Тесты не полагаются на фиксированные UID:
актуальные значения запрашиваются у приложения через HTTP, а сложные выборки
выполняются серверными скриптами (`Source/Lua`). Тесты устойчивы к изменениям
данных на стенде.

**Разделение времени жизни объектов.** Браузер живёт на класс тестов,
контекст и страница — на отдельный тест. Исключает взаимное влияние
и позволяет запускать классы параллельно.

---

## Запуск

### 1. Конфигурация

```bash
cp .env.example .env
cp crew.example.json crew.json
cp UITests/BaseProject/allureConfig.example.json UITests/BaseProject/allureConfig.json
```

| Файл | Содержимое |
|---|---|
| `.env` | Адрес стенда, учётная запись, ФИО сотрудника, внутренние эндпоинты |
| `crew.json` | Участники бригады для сценариев нарядов |

Все три файла находятся в `.gitignore`.

### 2. Браузер

```bash
dotnet build UITests/UIAutoTests.slnx
pwsh UITests/DefectsTests/bin/Debug/net10.0/playwright.ps1 install chromium
```

### 3. Тесты

Все модули:

```bash
dotnet test UITests/UIAutoTests.slnx
```

Отдельный модуль:

```bash
dotnet test UITests/DefectsTests/DefectsTests.csproj
```

Один сценарий:

```bash
dotnet test UITests/UIAutoTests.slnx --filter "FullyQualifiedName~CreateOrder"
```

### 4. Отчёт

```bash
allure serve UITests/AllureResults
```

---

## Отчётность

`AllureBaseTest` реализует единый жизненный цикл прогона:

- архивирование результатов предыдущего запуска с ограничением по глубине;
- мьютекс от гонок при параллельных запусках в нескольких терминалах;
- автоматическая классификация падений по `categories.json`
  (дефект продукта, проблема локатора, недоступность стенда, таймаут);
- передача аргументов `DataRow` и `DynamicData` в отчёт через AspectInjector;
- корректная очистка ресурсов: сбой закрытия браузера не маскирует
  реальный результат теста.

---

## Приватность данных

Репозиторий не содержит данных реальных пользователей:

| Что | Где хранится |
|---|---|
| Учётные данные, адрес стенда | `.env` |
| ФИО сотрудников | `crew.json`, `.env` |
| Внутренние адреса эндпоинтов | `.env` |
| UID объектов модели | подставляются через API в рантайме |
| Отчёты Allure (скриншоты, DOM, тела запросов) | `.gitignore` |

Тесты предназначены для стенда; для их запуска нужны заполненные `.env`
и `crew.json`.

---

## Лицензия

MIT — см. [LICENSE](LICENSE).