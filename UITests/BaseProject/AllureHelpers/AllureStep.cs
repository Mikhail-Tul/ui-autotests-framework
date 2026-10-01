using Allure.Net.Commons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace BaseProject.AllureHelpers
{
    public static class Step
    {
        /// <summary>
        /// Начать шаг с указанным именем
        /// </summary>
        public static void Start(string name)
        {
            ExtendedApi.StartStep(name);
        }

        /// <summary>
        /// Завершить шаг успешно
        /// </summary>
        public static void Pass()
        {
            ExtendedApi.PassStep();
        }

        /// <summary>
        /// Завершить шаг с ошибкой (и провалить тест)
        /// </summary>
        public static void Fail(string message = null)
        {
            ExtendedApi.FailStep();
            if (message != null)
                Assert.Fail(message);
        }

        /// <summary>
        /// Добавить ОР (ожидаемый результат) к текущему шагу как вложение
        /// </summary>
        public static void Expect(string expected, string actual, bool success)
        {
            string text = $"Ожидание: {expected}\n" +
                          $"Факт: {actual}\n" +
                          $"Результат: {(success ? "✅ Выполнено" : "❌ Не выполнено")}";
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            AllureApi.AddAttachment(
                name: "ОР",
                type: "text/plain",
                content: bytes,
                fileExtension: ".txt"
            );
        }
    }
}