using BaseProject.Base;
using BaseProject.Extensions;
using BaseProject.Helpers;

namespace WorkOrdersTests.Extensions
{
    /// <summary>
    /// Запрос данных, специфичных для нарядов.
    /// Обращается к тем же эндпоинтам, что и BaseProject, поэтому
    /// переиспользует общий ExecuteLuaScriptAsync вместо дублирования запроса.
    /// </summary>
    public static class APIExtensions
    {
        /// <summary>
        /// Возвращает наименование журнала учёта для текущего стенда.
        /// </summary>
        public static async Task<string> GetAccountingLog(this IAPIRequestContext API)
        {
            var result = await API.ExecuteLuaScriptAsync("Source/Lua/GetAccountingLog.lua");

            if (!result.TryGetProperty("value", out var element))
            {
                throw new InvalidOperationException(
                    "Данные: в ответе API нет свойства value (журнал учёта)");
            }

            var accountingLog = element.EnumerateArray()
                .Select(item => item.ToString())
                .LastOrDefault();

            if (string.IsNullOrWhiteSpace(accountingLog))
            {
                throw new InvalidOperationException(
                    "Данные: пустой журнал учёта (GetAccountingLog)");
            }

            return accountingLog;
        }
    }
}