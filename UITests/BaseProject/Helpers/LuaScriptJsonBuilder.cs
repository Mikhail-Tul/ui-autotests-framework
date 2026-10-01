using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BaseProject.Helpers
{
    /// <summary>
    /// Класс-строитель для создания JSON-представления Lua-скриптов с возможностью подстановки параметров.
    /// </summary>
    public sealed class LuaScriptJsonBuilder
    {
        private readonly string _luaTemplate;
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// Закрытый конструктор для инициализации строителя шаблоном Lua.
        /// </summary>
        /// <param name="luaTemplate">Шаблон Lua-скрипта.</param>
        private LuaScriptJsonBuilder(string luaTemplate)
        {
            _luaTemplate = luaTemplate ?? throw new ArgumentNullException(nameof(luaTemplate));
        }

        /// <summary>
        /// Создает экземпляр <see cref="LuaScriptJsonBuilder"/>, загружая шаблон из указанного файла.
        /// </summary>
        /// <param name="luaFilePath">Путь к файлу с Lua-скриптом.</param>
        /// <returns>Экземпляр строителя Lua-скрипта.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если путь к файлу пуст.</exception>
        /// <exception cref="FileNotFoundException">Выбрасывается, если файл не найден.</exception>
        public static LuaScriptJsonBuilder CreateFromFile(string luaFilePath)
        {
            if (string.IsNullOrWhiteSpace(luaFilePath))
            {
                throw new ArgumentException("Путь к Lua-скрипту не может быть пустым.", nameof(luaFilePath));
            }

            if (!File.Exists(luaFilePath))
            {
                throw new FileNotFoundException($"Не найден Lua-скрипт: {luaFilePath}", luaFilePath);
            }

            string template = File.ReadAllText(luaFilePath, Encoding.UTF8);

            return new LuaScriptJsonBuilder(template);
        }

        /// <summary>
        /// Добавляет параметр для подстановки в шаблон.
        /// </summary>
        /// <param name="name">Имя параметра (будет искаться в виде ${name}).</param>
        /// <param name="value">Значение параметра.</param>
        /// <returns>Текущий экземпляр строителя для цепочки вызовов.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если имя параметра пустое.</exception>
        public LuaScriptJsonBuilder WithParameter(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя параметра не может быть пустым.", nameof(name));
            }

            _parameters[name] = value ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Добавляет несколько параметров для подстановки в шаблон.
        /// </summary>
        /// <param name="parameters">Словарь с именами и значениями параметров.</param>
        /// <returns>Текущий экземпляр строителя для цепочки вызовов.</returns>
        public LuaScriptJsonBuilder WithParameters(IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return this;
            }

            foreach (var parameter in parameters)
            {
                WithParameter(parameter.Key, parameter.Value);
            }

            return this;
        }

        /// <summary>
        /// Формирует итоговую JSON-строку, содержащую обработанный Lua-скрипт.
        /// </summary>
        /// <returns>JSON-строка в формате {"luaScript": "..."}.</returns>
        public string Build()
        {
            string luaScript = ApplyParameters(_luaTemplate);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };

            return JsonSerializer.Serialize(new { luaScript = luaScript }, options);
        }

        /// <summary>
        /// Формирует JSON и сохраняет его в указанный файл в кодировке UTF-8 без BOM.
        /// </summary>
        /// <param name="outputPath">Путь к выходному JSON-файлу.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если путь к файлу пуст.</exception>
        public void BuildToFile(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Путь для сохранения JSON не может быть пустым.", nameof(outputPath));
            }

            string json = Build();

            // UTF8 без BOM
            File.WriteAllText(outputPath, json, new UTF8Encoding(false));
        }

        /// <summary>
        /// Заменяет плейсхолдеры в шаблоне на соответствующие значения параметров.
        /// </summary>
        /// <param name="template">Исходный шаблон Lua.</param>
        /// <returns>Шаблон с подставленными значениями.</returns>
        private string ApplyParameters(string template)
        {
            string result = template;

            foreach (var parameter in _parameters)
            {
                string placeholder = "${" + parameter.Key + "}";
                string value = EscapeLua(parameter.Value);

                result = result.Replace(placeholder, value);
            }

            return result;
        }

        /// <summary>
        /// Экранирует специальные символы строки для корректного использования в Lua-строковом литерале.
        /// </summary>
        /// <param name="value">Строка, которую необходимо экранировать.</param>
        /// <returns>Экранированная строка.</returns>
        private static string EscapeLua(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
