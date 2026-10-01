using System.Text.Json;
using System.Text.Json.Nodes;

namespace BaseProject.Helpers;

/// <summary>
/// Универсальный конструктор JSON по паттерну Fluent Builder.
/// Позволяет собирать JSON-объекты цепочкой вызовов.
///
/// Пример использования:
/// <code>
/// string json = new JsonBuilder()
///     .Add("name", "Иван")
///     .Add("age", 30)
///     .AddObject("address", b => b
///         .Add("city", "Москва")
///         .Add("zip", 123456))
///     .AddArray("roles", "admin", "user")
///     .ToJson();
/// </code>
/// </summary>
public sealed class JsonBuilder
{
    /// <summary>
    /// Внутреннее хранилище — JSON-объект, который мы собираем.
    /// </summary>
    private readonly JsonObject _json;

    /// <summary>
    /// Параметры сериализации по умолчанию (с отступами для читаемости).
    /// </summary>
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = null, // оставляем имена как есть
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Создаёт новый пустой конструктор.
    /// </summary>
    public JsonBuilder()
    {
        _json = new JsonObject();
    }

    /// <summary>
    /// Создаёт конструктор на основе уже существующего JSON-объекта.
    /// Полезно, если нужно дополнить уже готовый объект.
    /// </summary>
    /// <param name="existing">Существующий JSON-объект для доработки.</param>
    public JsonBuilder(JsonObject existing)
    {
        _json = existing ?? throw new ArgumentNullException(nameof(existing));
    }

    /// <summary>
    /// Создаёт конструктор из JSON-строки.
    /// Если строка невалидна — выбросит исключение.
    /// </summary>
    /// <param name="jsonString">Строка в формате JSON.</param>
    /// <returns>Новый экземпляр <see cref="JsonBuilder"/>.</returns>
    public static JsonBuilder FromJson(string jsonString)
    {
        var node = JsonNode.Parse(jsonString)
            ?? throw new ArgumentException("Не удалось распарсить JSON.", nameof(jsonString));

        var obj = node as JsonObject
            ?? throw new ArgumentException("Ожидался JSON-объект (фигурные скобки).", nameof(jsonString));

        return new JsonBuilder(obj);
    }

    // ──────────────────────────────────────────────
    //  Добавление примитивных значений
    // ──────────────────────────────────────────────

    /// <summary>
    /// Добавляет строковое значение.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение. Может быть null.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.Add("name", "Иван")</code>
    /// Результат: {"name": "Иван"}
    /// </example>
    public JsonBuilder Add(string key, string? value)
    {
        _json[key] = value is null ? null : JsonValue.Create(value);
        return this;
    }

    /// <summary>
    /// Добавляет целочисленное значение (int).
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.Add("age", 30)</code>
    /// Результат: {"age": 30}
    /// </example>
    public JsonBuilder Add(string key, int value)
    {
        _json[key] = JsonValue.Create(value);
        return this;
    }

    /// <summary>
    /// Добавляет целочисленное значение (long).
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder Add(string key, long value)
    {
        _json[key] = JsonValue.Create(value);
        return this;
    }

    /// <summary>
    /// Добавляет значение с плавающей точкой (double).
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.Add("voltage", 220.5)</code>
    /// Результат: {"voltage": 220.5}
    /// </example>
    public JsonBuilder Add(string key, double value)
    {
        _json[key] = JsonValue.Create(value);
        return this;
    }

    /// <summary>
    /// Добавляет значение с плавающей точкой (decimal).
    /// Подходит для денежных значений и точных измерений.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder Add(string key, decimal value)
    {
        _json[key] = JsonValue.Create(value);
        return this;
    }

    /// <summary>
    /// Добавляет булево значение (true / false).
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.Add("isActive", true)</code>
    /// Результат: {"isActive": true}
    /// </example>
    public JsonBuilder Add(string key, bool value)
    {
        _json[key] = JsonValue.Create(value);
        return this;
    }

    /// <summary>
    /// Добавляет дату и время в формате ISO 8601.
    /// Пример: "2026-09-07T14:30:00Z"
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение даты.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.Add("createdAt", DateTime.UtcNow)</code>
    /// Результат: {"createdAt": "2026-09-07T14:30:00Z"}
    /// </example>
    public JsonBuilder Add(string key, DateTime value)
    {
        _json[key] = JsonValue.Create(value.ToString("o")); // ISO 8601
        return this;
    }

    /// <summary>
    /// Добавляет GUID как строку.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение GUID.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder Add(string key, Guid value)
    {
        _json[key] = JsonValue.Create(value.ToString());
        return this;
    }

    /// <summary>
    /// Добавляет явное null-значение.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.AddNull("deletedAt")</code>
    /// Результат: {"deletedAt": null}
    /// </example>
    public JsonBuilder AddNull(string key)
    {
        _json[key] = null;
        return this;
    }

    /// <summary>
    /// Добавляет произвольный JSON-узел (если нужно что-то нестандартное).
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="node">Любой <see cref="JsonNode"/>.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder Add(string key, JsonNode? node)
    {
        _json[key] = node;
        return this;
    }

    // ──────────────────────────────────────────────
    //  Добавление вложенных объектов
    // ──────────────────────────────────────────────

    /// <summary>
    /// Добавляет вложенный JSON-объект через делегат.
    /// Внутри делегата получаешь новый билдер и наполняешь его.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="configure">Делегат для наполнения вложенного объекта.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>
    /// builder.AddObject("address", b => b
    ///     .Add("city", "Москва")
    ///     .Add("street", "Ленина, 1"));
    /// </code>
    /// Результат: {"address": {"city": "Москва", "street": "Ленина, 1"}}
    /// </example>
    public JsonBuilder AddObject(string key, Action<JsonBuilder> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var inner = new JsonBuilder();
        configure(inner);
        _json[key] = inner.Build();
        return this;
    }

    /// <summary>
    /// Добавляет вложенный объект из существующего C#-объекта.
    /// Сериализует объект через System.Text.Json.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="obj">Любой C#-объект для сериализации.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>
    /// var address = new { city = "Москва", zip = 123456 };
    /// builder.AddObject("address", address);
    /// </code>
    /// </example>
    public JsonBuilder AddObject<T>(string key, T obj) where T : class
    {
        var node = JsonSerializer.SerializeToNode(obj);
        _json[key] = node;
        return this;
    }

    // ──────────────────────────────────────────────
    //  Добавление массивов
    // ──────────────────────────────────────────────

    /// <summary>
    /// Добавляет массив строк.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="values">Набор строковых значений.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.AddArray("tags", "power", "energy", "grid")</code>
    /// Результат: {"tags": ["power", "energy", "grid"]}
    /// </example>
    public JsonBuilder AddArray(string key, params string[] values)
    {
        var array = new JsonArray();
        foreach (var v in values)
            array.Add(v);
        _json[key] = array;
        return this;
    }

    /// <summary>
    /// Добавляет массив целых чисел.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="values">Набор целых чисел.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.AddArray("ids", 1, 2, 3)</code>
    /// Результат: {"ids": [1, 2, 3]}
    /// </example>
    public JsonBuilder AddArray(string key, params int[] values)
    {
        var array = new JsonArray();
        foreach (var v in values)
            array.Add(v);
        _json[key] = array;
        return this;
    }

    /// <summary>
    /// Добавляет массив чисел с плавающей точкой.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="values">Набор значений типа double.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.AddArray("readings", 220.1, 221.3, 219.8)</code>
    /// Результат: {"readings": [220.1, 221.3, 219.8]}
    /// </example>
    public JsonBuilder AddArray(string key, params double[] values)
    {
        var array = new JsonArray();
        foreach (var v in values)
            array.Add(v);
        _json[key] = array;
        return this;
    }

    /// <summary>
    /// Добавляет массив булевых значений.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="values">Набор значений типа bool.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder AddArray(string key, params bool[] values)
    {
        var array = new JsonArray();
        foreach (var v in values)
            array.Add(v);
        _json[key] = array;
        return this;
    }

    /// <summary>
    /// Добавляет массив объектов, сериализованных из C#-объектов.
    /// </summary>
    /// <typeparam name="T">Тип элементов.</typeparam>
    /// <param name="key">Имя поля.</param>
    /// <param name="items">Коллекция объектов.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>
    /// var users = new[] { new { name = "A" }, new { name = "B" } };
    /// builder.AddArray("users", users);
    /// </code>
    /// </example>
    public JsonBuilder AddArray<T>(string key, IEnumerable<T> items)
    {
        var array = new JsonArray();
        foreach (var item in items)
            array.Add(JsonSerializer.SerializeToNode(item));
        _json[key] = array;
        return this;
    }

    /// <summary>
    /// Добавляет пустой массив. В JSON будет представлено как [].
    /// Не путать с null — поле существует, просто массив пустой.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.AddEmptyArray("items")</code>
    /// Результат: {"items": []}
    /// </example>
    public JsonBuilder AddEmptyArray(string key)
    {
        _json[key] = new JsonArray();
        return this;
    }

    /// <summary>
    /// Добавляет массив вложенных объектов через делегат.
    /// Каждый элемент массива собирается отдельным билдером.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="configureItems">
    /// Делегат, принимающий список билдеров.
    /// Добавь нужное количество элементов через <see cref="AddArrayItem"/>.
    /// </param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>
    /// builder.AddArrayOfObjects("meters", items =>
    /// {
    ///     items.AddArrayItem(b => b.Add("id", 1).Add("value", 100.5));
    ///     items.AddArrayItem(b => b.Add("id", 2).Add("value", 200.3));
    /// });
    /// </code>
    /// </example>
    public JsonBuilder AddArrayOfObjects(string key, Action<JsonArrayBuilder> configureItems)
    {
        if (configureItems is null)
            throw new ArgumentNullException(nameof(configureItems));

        var arrayBuilder = new JsonArrayBuilder();
        configureItems(arrayBuilder);
        _json[key] = arrayBuilder.Build();
        return this;
    }

    // ──────────────────────────────────────────────
    //  Манипуляции с полями
    // ──────────────────────────────────────────────

    /// <summary>
    /// Удаляет поле по ключу. Если поля нет — ничего не делает.
    /// </summary>
    /// <param name="key">Имя поля для удаления.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder Remove(string key)
    {
        _json.Remove(key);
        return this;
    }

    /// <summary>
    /// Добавляет поле только если условие истинно.
    /// Удобно для условных полей в тестах.
    /// </summary>
    /// <param name="condition">Условие добавления.</param>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    /// <example>
    /// <code>builder.AddIf(isAdmin, "role", "admin")</code>
    /// </example>
    public JsonBuilder AddIf(bool condition, string key, string? value)
    {
        if (condition)
            Add(key, value);
        return this;
    }

    /// <summary>
    /// Добавляет поле только если значение не равно null.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <param name="value">Значение (может быть null).</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder AddIfNotNull(string key, string? value)
    {
        if (value is not null)
            Add(key, value);
        return this;
    }

    /// <summary>
    /// Проверяет, содержит ли билдер указанное поле.
    /// </summary>
    /// <param name="key">Имя поля.</param>
    /// <returns>True, если поле существует.</returns>
    public bool Has(string key) => _json.ContainsKey(key);

    /// <summary>
    /// Объединяет текущий билдер с другим билдером.
    /// Поля из <paramref name="other"/> перезаписывают одноимённые поля текущего.
    /// </summary>
    /// <param name="other">Билдер, поля которого нужно добавить.</param>
    /// <returns>Текущий билдер для цепочки вызовов.</returns>
    public JsonBuilder Merge(JsonBuilder other)
    {
        foreach (var kvp in other.Build())
            _json[kvp.Key] = kvp.Value?.DeepClone();
        return this;
    }

    // ──────────────────────────────────────────────
    //  Финализация (получение результата)
    // ──────────────────────────────────────────────

    /// <summary>
    /// Возвращает собранный <see cref="JsonObject"/>.
    /// </summary>
    /// <returns>Готовый JSON-объект.</returns>
    public JsonObject Build() => _json;

    /// <summary>
    /// Сериализует собранный объект в JSON-строку.
    /// </summary>
    /// <param name="indented">
    /// Если true — форматированный вывод с отступами (для логирования).
    /// Если false — компактный (для отправки в API).
    /// По умолчанию: true.
    /// </param>
    /// <returns>JSON-строка.</returns>
    public string ToJson(bool indented = true)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        return _json.ToJsonString(options);
    }

    /// <summary>
    /// Неявное приведение к строке.
    /// Позволяет использовать билдер напрямую там, где ожидается string.
    /// </summary>
    /// <example>
    /// <code>
    /// string json = new JsonBuilder().Add("x", 1); // вызовет ToJson()
    /// </code>
    /// </example>
    public static implicit operator string(JsonBuilder builder) => builder.ToJson();

    /// <summary>
    /// Возвращает JSON-строку при вызове ToString().
    /// Удобно для логирования и отладки.
    /// </summary>
    public override string ToString() => ToJson();
}

// ══════════════════════════════════════════════════
//  Вспомогательный класс для построения массивов объектов
// ══════════════════════════════════════════════════

/// <summary>
/// Вспомогательный билдер для создания массивов из объектов.
/// Используется внутри <see cref="JsonBuilder.AddArrayOfObjects"/>.
/// Не предназначен для прямого создания.
/// </summary>
public sealed class JsonArrayBuilder
{
    /// <summary>
    /// Внутренний массив, который наполняется.
    /// </summary>
    private readonly JsonArray _array = new();

    /// <summary>
    /// Добавляет один объект в массив через делегат.
    /// </summary>
    /// <param name="configure">Делегат для наполнения одного элемента массива.</param>
    /// <returns>Текущий билдер массива для цепочки.</returns>
    /// <example>
    /// <code>
    /// items.AddArrayItem(b => b.Add("id", 1).Add("value", 42.0));
    /// </code>
    /// </example>
    public JsonArrayBuilder AddArrayItem(Action<JsonBuilder> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var builder = new JsonBuilder();
        configure(builder);
        _array.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Добавляет примитивное значение в массив.
    /// </summary>
    /// <param name="value">Значение для добавления.</param>
    /// <returns>Текущий билдер массива для цепочки.</returns>
    public JsonArrayBuilder AddValue(string value)
    {
        _array.Add(value);
        return this;
    }

    /// <summary>
    /// Добавляет целое число в массив.
    /// </summary>
    /// <param name="value">Значение для добавления.</param>
    /// <returns>Текущий билдер массива для цепочки.</returns>
    public JsonArrayBuilder AddValue(int value)
    {
        _array.Add(value);
        return this;
    }

    /// <summary>
    /// Добавляет дробное число в массив.
    /// </summary>
    /// <param name="value">Значение для добавления.</param>
    /// <returns>Текущий билдер массива для цепочки.</returns>
    public JsonArrayBuilder AddValue(double value)
    {
        _array.Add(value);
        return this;
    }

    /// <summary>
    /// Возвращает собранный массив.
    /// Вызывается автоматически внутри <see cref="JsonBuilder.AddArrayOfObjects"/>.
    /// </summary>
    /// <returns>Готовый <see cref="JsonArray"/>.</returns>
    internal JsonArray Build() => _array;
}