using System.Text.Json;

namespace BaseProject.Base
{
    /// <summary>
    /// Участники бригады для сценариев наряда-допуска и распоряжения.
    /// ФИО читаются из crew.json (см. crew.example.json), поэтому реальные
    /// сотрудники не фигурируют в исходном коде.
    /// </summary>
    public static class Crew
    {
        private sealed class Data
        {
            public string PermitIssuer { get; init; } = "";
            public string WorkSupervisor { get; init; } = "";
            public string WorkProducer { get; init; } = "";
            public string Observer { get; init; } = "";
            public string CrewMember { get; init; } = "";
        }

        private static readonly Lazy<Data> Instance = new(Load);

        private static Data Current => Instance.Value;

        /// <summary>Допускающий — выдаёт разрешение на работы.</summary>
        public static string PermitIssuer => Current.PermitIssuer;

        /// <summary>Руководитель работ.</summary>
        public static string WorkSupervisor => Current.WorkSupervisor;

        /// <summary>Производитель работ.</summary>
        public static string WorkProducer => Current.WorkProducer;

        /// <summary>Наблюдающий.</summary>
        public static string Observer => Current.Observer;

        /// <summary>Член бригады.</summary>
        public static string CrewMember => Current.CrewMember;

        private static Data Load()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "crew.json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"Файл crew.json не найден. Скопируйте crew.example.json в crew.json " +
                    "и укажите сотрудников вашего стенда.", path);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            var data = JsonSerializer.Deserialize<Data>(File.ReadAllText(path), options)
                ?? throw new InvalidOperationException($"Не удалось разобрать crew.json: {path}");

            Validate(data, path);
            return data;
        }

        private static void Validate(Data data, string path)
        {
            var missing = new List<string>();

            void Check(string name, string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    missing.Add(name);
                }
            }

            Check(nameof(data.PermitIssuer), data.PermitIssuer);
            Check(nameof(data.WorkSupervisor), data.WorkSupervisor);
            Check(nameof(data.WorkProducer), data.WorkProducer);
            Check(nameof(data.Observer), data.Observer);
            Check(nameof(data.CrewMember), data.CrewMember);

            if (missing.Count > 0)
            {
                throw new InvalidOperationException(
                    $"В crew.json не заполнены поля: {string.Join(", ", missing)}. Файл: {path}");
            }
        }
    }
}