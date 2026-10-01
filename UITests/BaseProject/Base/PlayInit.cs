using Allure.Net.Commons.Attributes;
using Microsoft.Playwright;

namespace BaseProject.Base
{
    [TestClass]
    /// <summary>
    /// Базовый класс для инициализации Playwright.
    /// Обеспечивает создание браузера, контекстов и страницы для каждого теста.
    /// </summary>
    public class PlayInit : BaseClass
    {
        // Эти объекты живут на весь класс тестов
        private static IPlaywright _playwright;
        private static IBrowser _browser;

        // Эти объекты создаются для каждого теста
        /// <summary>
        /// Контекст API-запросов для текущего теста.
        /// </summary>
        protected IAPIRequestContext APIContext;

        /// <summary>
        /// Контекст браузера (куки, кэш) для текущего теста.
        /// </summary>
        protected IBrowserContext BrowserContext;

        /// <summary>
        /// Страница браузера для текущего теста.
        /// </summary>
        protected IPage Page;

        /// <summary>
        /// Подготавливает API контекст, контекст браузера и новую страницу перед каждым тестом.
        /// </summary>
        [TestInitialize]
        [AllureBefore]
        public async Task PlayIni()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Headless = Config.Headless
            });
            // 1. API контекст для этого теста
            APIContext = await _playwright.APIRequest.NewContextAsync(new()
            {
                HttpCredentials = new()
                {
                    Username = Config.Login,
                    Password = Config.Password,
                },
                IgnoreHTTPSErrors = true,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            });
            await APIContext.PostAsync($"{Config.BaseUrl}/auth/app/token");

            // 2. НОВЫЙ контекст браузера для этого теста (чистые куки и кэш)
            BrowserContext = await _browser.NewContextAsync(new()
            {
                ViewportSize = new()
                {
                    Height = 1080,
                    Width = 1920
                },
                HttpCredentials = new()
                {
                    Username = Config.Login,
                    Password = Config.Password,
                }
            });

            // 3. НОВАЯ страница для этого теста
            Page = await BrowserContext.NewPageAsync();
        }

        /// <summary>
        /// Выполняет авторизацию в указанном модуле системы.
        /// </summary>
        /// <param name="modulePath">Путь к модулю (например, "defects").</param>
        protected async Task AuthorizeAsync(string modulePath)
        {
            await Page.GotoAsync($"{Config.BaseUrl}/{modulePath}/");
            await Page.Locator("//div[@onclick='authWindows()']").ClickAsync();
        }

        /// <summary>
        /// Закрывает ресурсы теста (страницу, контексты).
        /// </summary>
        [TestCleanup]
        public async Task Close()
        {
            try
            {
                if (Page != null) await Page.CloseAsync();
                if (BrowserContext != null) await BrowserContext.DisposeAsync();
                if (APIContext != null) await APIContext.DisposeAsync();
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                }
                _playwright?.Dispose();
            }
            catch (Exception ex)
            {
                // Сбой очистки (например, контекст уже закрыт навигацией) не должен
                // маскировать реальный исход теста.
                System.Diagnostics.Debug.WriteLine($"[PlayInit] Ошибка очистки: {ex.Message}");
            }
        }
    }
}