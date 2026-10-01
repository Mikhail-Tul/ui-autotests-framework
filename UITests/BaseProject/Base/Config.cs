namespace BaseProject.Base
{
    /// <summary>
    /// Конфигурация тестового окружения.
    /// Все значения читаются из переменных окружения с префиксом UITEST_ (см. .env.example),
    /// поэтому секреты и адреса внутренних стендов не попадают в репозиторий.
    /// </summary>
    public static class Config
    {
        /// <summary>Базовый адрес тестируемого приложения, например https://stand.example.com</summary>
        public static string BaseUrl => Required("BASE_URL");

        /// <summary>Логин пользователя тестового стенда.</summary>
        public static string Login => Optional("LOGIN");

        /// <summary>Пароль пользователя тестового стенда. Никогда не попадает в отчёт Allure.</summary>
        public static string Password => Optional("PASSWORD");

        /// <summary>ФИО сотрудника, от имени которого выполняются тесты. Используется как ключ поиска в справочниках.</summary>
        public static string PersonName => Required("PERSON_NAME");

        /// <summary>Наименование подразделения сотрудника — для проверки дерева оргструктуры.</summary>
        public static string PersonsDepartment => Required("PERSON_DEPARTMENT");

        /// <summary>Роль сотрудника в смене (например «Дежурный персонал»).</summary>
        public static string ShiftRole => Required("SHIFT_ROLE");

        /// <summary>Произвольный текст-шаблон для полей ввода: проверяет ввод, ретраит и обрезку значения.</summary>
        public static string TextPattern => Optional("TEXT_PATTERN", "Проверка ввода автотеста 1234");

        /// <summary>Запускать ли браузер без окна. По умолчанию — нет.</summary>
        public static bool Headless => Bool("HEADLESS", false);

        /// <summary>
        /// Путь к эндпоинту, который отдаёт актуальные UID объектов модели.
        /// Относится к внутреннему контракту системы, поэтому задаётся
        /// в окружении, а не зашивается в исходный код.
        /// </summary>
        public static string ObjectModelsApiPath => Optional("OBJECT_MODELS_API_PATH");

        /// <summary>Путь к эндпоинту выполнения серверного скрипта.</summary>
        public static string ExecuteScriptApiPath => Optional("EXECUTE_SCRIPT_API_PATH");

        /// <summary>
        /// Путь к эндпоинту смены. Операция изменяет состояние процесса,
        /// поэтому её адрес также вынесен в окружение.
        /// </summary>
        public static string ShiftChangeApiPath => Optional("SHIFT_CHANGE_API_PATH");

        /// <summary>
        /// Базовый путь к справочникам модуля осмотров.
        /// Тесты создают и удаляют в них записи, поэтому адрес вынесен в окружение.
        /// </summary>
        public static string DirectoriesApiPath => Optional("DIRECTORIES_API_PATH");

        /// <summary>
        /// Обязательная переменная окружения. Если она не задана, тест падает
        /// с понятным сообщением вместо NullReferenceException в середине сценария.
        /// </summary>
        private static string Required(string name)
        {
            var value = Environment.GetEnvironmentVariable(Prefix + name);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Переменная окружения {Prefix}{name} не задана. " +
                    "Скопируйте .env.example в .env и заполните значения.");
            }

            return value;
        }

        private static string Optional(string name, string fallback = "")
        {
            var value = Environment.GetEnvironmentVariable(Prefix + name);
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static bool Bool(string name, bool fallback)
        {
            var value = Environment.GetEnvironmentVariable(Prefix + name);

            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return bool.TryParse(value, out var parsed)
                ? parsed
                : throw new InvalidOperationException(
                    $"Переменная окружения {Prefix}{name} должна быть true или false, получено «{value}».");
        }

        private const string Prefix = "UITEST_";
    }
}