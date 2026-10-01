using BaseProject.Base;
using BaseProject.Extensions;

namespace DispJournalTests.Extensions
{
    /// <summary>
    /// Запрос данных справочников оперативного журнала.
    ///
    /// Обращается к тем же эндпоинтам, что и <see cref="APIExtensions"/> из
    /// BaseProject, поэтому переиспользует общий метод ExecuteLuaScriptAsync
    /// и не дублирует сборку запроса и разбор ответа.
    /// </summary>
    public static class DispAPIExtensions
    {
        /// <summary>
        /// Возвращает типы категорий из справочника.
        /// </summary>
        public static async Task<List<string>> GetCategoryTypeAsync(this IAPIRequestContext context)
        {
            var result = await context.ExecuteLuaScriptAsync("Source/Lua/GetDjCategoryType.lua");

            var values = ReadValues(result, "типов категорий");

            if (values.Count == 0)
            {
                throw new InvalidOperationException(
                    "Данные: пустой список типов категорий (GetCategoryTypeAsync)");
            }

            return values;
        }

        /// <summary>
        /// Возвращает категории указанного типа.
        /// </summary>
        public static async Task<List<string>> GetCategorysAsync(
            this IAPIRequestContext context,
            string categoryType)
        {
            var result = await context.ExecuteLuaScriptAsync(
                "Source/Lua/GetCategory.lua",
                new() { { "CategoryTypeName", categoryType } });

            var values = ReadValues(result, $"категорий типа «{categoryType}»");

            if (values.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Данные: пустой список категорий (GetCategorysAsync, тип: {categoryType})");
            }

            return values;
        }

        /// <summary>
        /// Преобразует элементы ответа API в список строк.
        /// </summary>
        private static List<string> ReadValues(System.Text.Json.JsonElement result, string what)
        {
            if (!result.TryGetProperty("value", out var element))
            {
                throw new InvalidOperationException(
                    $"Данные: в ответе API нет свойства value ({what})");
            }

            return element.EnumerateArray()
                .Select(item => item.ToString())
                .ToList();
        }
    }
}