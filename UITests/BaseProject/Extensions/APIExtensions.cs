using Microsoft.Playwright;
using BaseProject.Helpers;
using BaseProject.Base;

namespace BaseProject.Extensions
{
    public static class APIExtensions
    {
        private static string? _modelUid;

        /// <summary>
        /// Проверяет статус HTTP-ответа и бросает исключение с кодом и телом ответа при не-OK.
        /// </summary>
        private static async Task EnsureResponseOkAsync(IAPIResponse response, string action)
        {
            if (response.Ok) return;
            string body = "";
            try { body = await response.TextAsync(); } catch { /* тело недоступно */ }
            throw new InvalidOperationException(
                $"API: HTTP {(int)response.Status} при {action}. Тело ответа: {body}");
        }

        /// <summary>
        /// Требует, чтобы путь к эндпоинту был задан в окружении.
        /// Без понятной ошибки тест падал бы позже и с невнятным сообщением.
        /// </summary>
        private static string RequirePath(string? path, string variableName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException(
                    $"Не задан путь к эндпоинту. Укажите его в .env " +
                    $"(переменная UITEST_{variableName}).");
            }

            return path;
        }

        /// <summary>
        /// Получает актуальный UID модели объекта из системы.
        /// Адрес эндпоинта берётся из конфигурации окружения.
        /// </summary>
        /// <param name="API">Контекст API-запросов.</param>
        public static async Task<string> GetObjectModel(this IAPIRequestContext API)
        {
            var path = RequirePath(Config.ObjectModelsApiPath, "OBJECT_MODELS_API_PATH");

            var response = await API.GetAsync($"{Config.BaseUrl}{path}");
            await EnsureResponseOkAsync(response, "GET object-models");

            var json = await response.JsonAsync();

            if (json.Value.TryGetProperty("value", out var res))
            {
                foreach (var item in res.EnumerateArray())
                {
                    if (item.TryGetProperty("modelUid", out var temp))
                    {
                        _modelUid = temp.GetString();
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(_modelUid))
                throw new InvalidOperationException("Данные: не найден modelUid объекта в ответе API");

            return _modelUid;
        }

        /// <summary>
        /// Вспомогательный метод для выполнения Lua-скриптов через API.
        /// </summary>
        /// <param name="API">Контекст API-запросов.</param>
        /// <param name="scriptPath">Путь к Lua-скрипту.</param>
        /// <param name="parameters">Параметры для скрипта.</param>
        /// <returns>Результат выполнения скрипта в виде JsonElement.</returns>
        public static async Task<System.Text.Json.JsonElement> ExecuteLuaScriptAsync(this IAPIRequestContext API, string scriptPath, Dictionary<string, string>? parameters = null)
        {
            var modelUid = await API.GetObjectModel();
            var builder = LuaScriptJsonBuilder.CreateFromFile(scriptPath);
            if (parameters != null)
            {
                builder.WithParameters(parameters);
            }
            var json = builder.Build();

            var path = RequirePath(Config.ExecuteScriptApiPath, "EXECUTE_SCRIPT_API_PATH")
                    .Replace("{modelUid}", modelUid);

            var response = await API.PostAsync($"{Config.BaseUrl}{path}",
                new() { Data = json });
            await EnsureResponseOkAsync(response, $"execute-script {Path.GetFileName(scriptPath)}");
            var jsonEl = await response.JsonAsync();
            if (!jsonEl.Value.TryGetProperty("value", out var valueEl))
                throw new InvalidOperationException($"Данные: нет свойства value в ответе execute-script {Path.GetFileName(scriptPath)}");
            return valueEl;
        }

        /// <summary>
        /// Получает иерархию подразделений для указанного пользователя.
        /// </summary>
        /// <param name="API">Контекст API-запросов.</param>
        /// <param name="personTree">Список для заполнения данными о дереве подразделений.</param>
        /// <param name="name">Имя пользователя.</param>
        /// <returns>Заполненный список подразделений.</returns>
        public static async Task<List<string>> GetTreePerson(this IAPIRequestContext API, string name)
        {
            var personTree = new List<string>();
            var result = await API.ExecuteLuaScriptAsync("Source/Lua/GetPersonDepartmentChain.lua", new() { { "personName", name } });
            foreach (var item in result.EnumerateArray())
            {
                personTree.Add(item.ToString());
            }
            if (personTree.Count == 0)
                throw new InvalidOperationException($"Данные: пустой результат GetTreePerson (personName: {name})");
            return personTree;
        }

        /// <summary>
        /// Получает UID смен для указанного пользователя и роли.
        /// </summary>
        /// <param name="context">Контекст API-запросов.</param>
        /// <param name="shiftRole">Роль в смене.</param>
        /// <param name="personName">Имя пользователя.</param>
        /// <returns>Список UID смен.</returns>
        public static async Task<List<string>> GetShiftUidsAsync(this IAPIRequestContext context, string shiftRole, string personName)
        {
            var result = await context.ExecuteLuaScriptAsync("Source/Lua/GetShiftUids.lua", new()
            {
                { "personName", personName },
                { "shiftRoleName", shiftRole },
                { "shiftName", "Дневная смена" }
            });
            List<string> uids = new();
            foreach (var item in result.EnumerateArray())
            {
                uids.Add(item.ToString());
            }
            if (uids.Count < 3)
                throw new InvalidOperationException($"Данные: неполный результат GetShiftUids (ожидалось >= 3 uid, получено {uids.Count}; role: {shiftRole}, person: {personName})");
            return uids;
        }

        /// <summary>
        /// Получает название подстанции.
        /// </summary>
        /// <param name="API">Контекст API-запросов.</param>
        /// <returns>Название подстанции.</returns>
        public static async Task<string> GetSubstationName(this IAPIRequestContext API)
        {
            var result = await API.ExecuteLuaScriptAsync("Source/Lua/GetSubstationName.lua");
            string substationName = "";
            foreach (var item in result.EnumerateArray())
            {
                substationName = item.ToString();
            }
            if (string.IsNullOrEmpty(substationName))
                throw new InvalidOperationException("Данные: пустой результат GetSubstationName");
            return substationName;
        }

        /// <summary>
        /// Получает дерево структуры подстанции.
        /// </summary>
        /// <param name="API">Контекст API-запросов.</param>
        /// <param name="substationName">Название подстанции.</param>
        /// <returns>Список элементов дерева подстанции.</returns>
        public static async Task<List<string>> GetSubstationTree(this IAPIRequestContext API, string substationName)
        {
            var result = await API.ExecuteLuaScriptAsync("Source/Lua/GetSubstationTree.lua", new() { { "substationName", substationName } });
            List<string> substationOrgTree = new();
            foreach (var item in result.EnumerateArray())
            {
                substationOrgTree.Add(item.ToString());
            }
            if (substationOrgTree.Count == 0)
                throw new InvalidOperationException($"Данные: пустой результат GetSubstationTree (substationName: {substationName})");
            return substationOrgTree;
        }

        /// <summary>
        /// Получает название оборудования на подстанции.
        /// </summary>
        /// <param name="API">Контекст API-запросов.</param>
        /// <param name="substationName">Название подстанции.</param>
        /// <returns>Название оборудования.</returns>
        public static async Task<string> GetEquipmentName(this IAPIRequestContext API, string substationName)
        {
            var result = await API.ExecuteLuaScriptAsync("Source/Lua/GetSubstationEquipmentName.lua", new() { { "substationName", substationName } });
            string equipmentName = "";
            foreach (var item in result.EnumerateArray())
            {
                equipmentName = item.GetString();
            }
            if (string.IsNullOrEmpty(equipmentName))
                throw new InvalidOperationException($"Данные: пустой результат GetEquipmentName (substationName: {substationName})");
            return equipmentName;
        }
        /// <summary>
        /// Получает список типов дефектов.
        /// </summary>
        /// <param name="API">Контекст API-запросов.</param>
        /// <returns>Список типов дефектов.</returns>
        public static async Task<List<string>> GetDefectTypesAsync(this IAPIRequestContext API)
        {
        var result = await API.ExecuteLuaScriptAsync("Source/Lua/GetTypeOfDefect.lua");

        var defectTypes = result.EnumerateArray()
            .Select(item => item.GetString() ?? "")
            .ToList();

        if (defectTypes.Count == 0)
            throw new InvalidOperationException("Данные: пустой список типов дефектов (GetDefectTypesAsync)");

        return defectTypes;
    }
}
}
