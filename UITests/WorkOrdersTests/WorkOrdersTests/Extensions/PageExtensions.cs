using WorkOrdersTests.Map;
using static Microsoft.Playwright.Assertions;
using System.Text.RegularExpressions;
using System.Globalization;
using BaseProject.Extensions;
using BaseProject.Base;

namespace WorkOrdersTests.Extensions
{
    /// <summary>
    /// Класс расширений для страницы, предоставляющий вспомогательные методы для работы с элементами интерфейса приложения.
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Выбирает сотрудника в окне выбора персонала, используя иерархию дерева подразделений.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="tree">Список подразделений для раскрытия в дереве.</param>
        /// <param name="person">Роль сотрудника (например, "члена бригады").</param>
        /// <param name="personName">Имя сотрудника для выбора.</param>
        public static async Task ChoicePersonInWindowAsync(this IPage page, List<string> tree, string person, string personName)
        {
            ChoicePersonWindow choicePersonWindow = new ChoicePersonWindow(page);
            PersonComboBox personComboBox = new PersonComboBox(page);
            ILocator loc;

            loc = await AllureApi.Step<ILocator>("Нажать кнопку выбора из справочника", async () =>
            {
                if (person == "члена бригады" || person == "согласующего")
                {
                    loc = page.Locator($"//app-personal-list[@placeholderstatic='Выберите {person}']");
                    await Expect(loc).ToBeVisibleAsync();
                    await Expect(loc.GetByTitle("Добавить из справочника")).ToBeVisibleAsync();
                    await loc.GetByTitle("Добавить из справочника").ClickAsync();
                    await page.WaitForRequestFinishedAsync(new()
                    {
                        Predicate = r => r.Url.Contains("persons-tree/items/query")
                    });
                    return loc;
                }
                else
                {
                    loc = page.Locator($"//app-person-selector[@placeholderstatic='Выберите {person}']");
                    await Expect(loc).ToBeVisibleAsync();
                    await loc.HoverAsync();
                    await Expect(loc.GetByTitle("Выбор из справочника")).ToBeVisibleAsync();
                    await loc.GetByTitle("Выбор из справочника").ClickAsync();
                    await page.WaitForRequestFinishedAsync(new()
                    {
                        Predicate = r => r.Url.Contains("persons-tree/items/query")
                    });
                    return loc;
                }
            });
            await AllureApi.Step("Проверить отображение дерева персонала", async () =>
            {
                foreach (var item in tree)
                {
                    try
                    {
                        await page.GetByRole(AriaRole.Gridcell, new() { Name = item }).ClickAsync();
                    }
                    catch (Exception ex) when (ex is not AssertFailedException)
                    {
                        throw new InvalidOperationException(
                            $"Шаг «Проверить отображение дерева персонала»: элемент «{item}»: {ex.Message}", ex);
                    }
                }
            });
            await AllureApi.Step("Выбрать персонал", async () =>
            {
                await page.ScrollWhileUnvisibleAsync(
                    page.GetByRole(AriaRole.Gridcell, new() { Name = personName }).
                    Filter(new() { Has = page.Locator("img") }),
                    page.Locator(".dx-treelist-rowsview").Locator(".dx-scrollable-container"));

                var count = await page.GetByRole(AriaRole.Gridcell, new() { Name = personName })
                    .Filter(new() { Has = page.Locator("img") })
                    .GetByRole(AriaRole.Checkbox).CountAsync();

                if (count > 0)
                {
                    await page.GetByRole(AriaRole.Gridcell, new() { Name = personName }).
                        Filter(new() { Has = page.Locator("img") }).GetByRole(AriaRole.Checkbox).
                        CheckAsync();
                }
                else
                {
                    await page.GetByRole(AriaRole.Gridcell, new() { Name = personName })
                        .Filter(new() { Has = page.Locator("img") })
                        .ClickAsync();
                }
            });
            await AllureApi.Step("Подтвердить выбор", async () =>
            {
                await choicePersonWindow.ChoiceButton.ClickAsync();
                await Expect(choicePersonWindow.ChoiceButton).ToBeVisibleAsync(new() { Visible = false });
                await Expect(loc.GetByRole(AriaRole.Img)).ToBeVisibleAsync();
            });
            await AllureApi.Step("Для выбранного персонала выставить группу ЭБ", async () =>
            {
                await Expect(loc.GetByRole(AriaRole.Combobox)).ToBeVisibleAsync(new() { Timeout = 2000 });
                await loc.GetByRole(AriaRole.Combobox).ClickAsync();
                await personComboBox.GREB.ClickAsync();
                await Expect(personComboBox.GREB).ToBeVisibleAsync(new() { Visible = false });
            });
        }

        /// <summary>
        /// Добавляет объект в заявку, используя иерархию дерева подстанции.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="substationTree">Список элементов дерева подстанции для навигации.</param>
        public static async Task AddObjectAsync(this IPage page, List<string> substationTree)
        {

            CreateWindow euWin = new(page);
            ChoiceObjectWindow choiceObjectWindow = new(page);
            await AllureApi.Step("Нажать кнопку \"Выбрать из справочника\"", async () =>
            {
                await euWin.AddObjectButton.ClickAsync();
            });
            await AllureApi.Step("Переключить на полное дерево", async () =>
            {
                await choiceObjectWindow.FullTreeSwitch.ClickAsync();
                await page.WaitForRequestFinishedAsync(new()
                {
                    Predicate = r => r.Url.Contains("api/tree/parentchild/") && !r.Url.Contains("OnlyUserAOR=true"),
                    Timeout = 300000
                });
            });
            await AllureApi.Step("Раскрыть дерево и выбрать объект", async () =>
            {
                await page.SelectObjectAsync(substationTree);
            });
            await AllureApi.Step("Подвердить выбор объекта", async () =>
            {
                await choiceObjectWindow.SaveButton.ClickAsync();

                await Expect(choiceObjectWindow.SaveButton).ToBeVisibleAsync(new() { Visible = false });
            });
        }

        /// <summary>
        /// Добавляет место проведения работ в заявку.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="substationName">Название подстанции.</param>
        /// <param name="equipmentName">Название оборудования.</param>
        public static async Task AddPlaceWorkAsync(this IPage page, string substationName, string equipmentName)
        {
            CreateWindow eUWindow = new(page);
            ChoiceObjectWindow choiceObjectWindow = new(page);
            await AllureApi.Step("Нажать кнопку \"Добавить из справочника\"", async () =>
            {
                await Expect(eUWindow.AddPlaceWork).ToBeVisibleAsync();

                await eUWindow.AddPlaceWorkButton.ClickAsync();

                await page.WaitForRequestFinishedAsync(new()
                {
                    Predicate = r => r.Url.Contains("lineartree")
                });
            });
            await AllureApi.Step("Раскрываю подстанцию", async () =>
            {
                await Expect(page.BuildCollapsedButton(substationName)).ToBeInViewportAsync(new() { Timeout = 10000 });
                await page.BuildCollapsedButton(substationName).ClickAsync();
            });
            await AllureApi.Step("Выбираю оборудование", async () =>
            {
                await Expect(page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new($@".\s{equipmentName}\s.", RegexOptions.IgnoreCase) }).
                    GetByRole(AriaRole.Checkbox).First).ToBeVisibleAsync(new() { Timeout = 5000 });
                await page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new($@".\s{equipmentName}\s.", RegexOptions.IgnoreCase) }).
                    GetByRole(AriaRole.Checkbox).First.ClickAsync();
            });
            await AllureApi.Step("Подтверждаю выбор", async () =>
            {
                await choiceObjectWindow.SaveButton.ClickAsync();

                await Expect(choiceObjectWindow.SaveButton).ToBeVisibleAsync(new() { Visible = false });
            });
            await AllureApi.Step("Проверяю отображение оборудования", async () =>
            {
                await Expect(page.GetByTitle(equipmentName, new() { Exact = true })).ToBeVisibleAsync();
            });
        }

        /// <summary>
        /// Заполняет текстовое содержимое работ в заявке стандартными значениями.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        public static async Task FillTheContentOfTheWorksAsync(this IPage page)
        {
            CreateWindow eUWindow = new(page);

            await Expect(eUWindow.TheContentOfTheWorksTextInput).ToBeVisibleAsync();
            await Expect(eUWindow.AddObjectInContentOfTheWorkButton).ToBeVisibleAsync();
            await Expect(eUWindow.AddWorkPlaceInContentOfTheWorkButton).ToBeVisibleAsync();

            await eUWindow.TheContentOfTheWorksTextInput.ClickAsync();
            await eUWindow.TheContentOfTheWorksTextInput.PressSequentiallyAsync(Config.TextPattern);
            await eUWindow.AddObjectInContentOfTheWorkButton.ClickAsync();
            await eUWindow.AddWorkPlaceInContentOfTheWorkButton.ClickAsync();
        }

        /// <summary>
        /// Устанавливает дату начала или окончания работ через календарь.
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="start">Истина, если устанавливается дата начала; иначе - дата окончания.</param>
        /// <param name="date">Дата для установки.</param>
        public static async Task FillDatesAsync(this IPage page, bool start, DateTime date)
        {
            CreateWindow eUWindow = new(page);
            CalendarEl calendar = new(page);

            if (start)
            {
                await Expect(eUWindow.TimeStartWorksPlaceholder).ToBeVisibleAsync();
                await Expect(eUWindow.TimeStartWorksCalendarButton).ToBeVisibleAsync();
                await eUWindow.TimeStartWorksCalendarButton.ClickAsync();
            }
            else
            {
                await Expect(eUWindow.TimeEndWorksPlaceholder).ToBeVisibleAsync();
                await Expect(eUWindow.TimeEndWorksCalendarButton).ToBeVisibleAsync();
                await eUWindow.TimeEndWorksCalendarButton.ClickAsync();
            }

            await Expect(calendar.CalendarPopUp).ToBeVisibleAsync();
            await Expect(calendar.CalendarDates).ToBeVisibleAsync();
            await Expect(calendar.CalendarTimes).ToBeVisibleAsync();
            await Expect(calendar.OKButton).ToBeVisibleAsync();
            await Expect(calendar.HoureUpButton).ToBeVisibleAsync();
            await Expect(calendar.MinuteUpButton).ToBeVisibleAsync();

            await calendar.DateInCalendar.GetByLabel(await page.GetFormatDateAsync(date)).First.ClickAsync();
            await calendar.HoureUpButton.ClickAsync(new() { ClickCount = 5 });
            await calendar.MinuteUpButton.ClickAsync(new() { ClickCount = 5 });
            await calendar.OKButton.ClickAsync();

            await Expect(calendar.OKButton).ToBeVisibleAsync(new() { Visible = false });
        }

        /// <summary>
        /// Форматирует дату в строку, соответствующую формату календаря приложения (на русском языке).
        /// </summary>
        /// <param name="page">Страница браузера.</param>
        /// <param name="date">Дата для форматирования.</param>
        /// <returns>Отформатированная строка даты.</returns>
        public static async Task<string> GetFormatDateAsync(this IPage page, DateTime date)
        {
            string format = date.ToString("dddd, d MMMM yyyy г.", new CultureInfo("ru-RU"));

            return format;
        }
    }
}
