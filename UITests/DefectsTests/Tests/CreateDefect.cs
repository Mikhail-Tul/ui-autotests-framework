using BaseProject.Base;
using DefectsTests.BaseDefects;
using BaseProject.AllureHelpers;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons;

namespace DefectsTests.Tests
{
    [TestClass]
    [BaseAttributeSmoke]
    [DataRowArgsAspect]
    [AllureBddHierarchy(
        "Defects",
        "Создание записи",
        "Создание записи")]
    [AllureSuiteHierarchy(
        "Defect - (Дефекты)",
        "Создание записи")]
    public class CreateDefects : Auth
    {
        [AllureBefore]
        public static IEnumerable<object[]> GetDefectTypes()
        {
            IPlaywright playwright = null;
            IAPIRequestContext apiContext = null;
            try
            {
                playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
                apiContext = playwright.APIRequest.NewContextAsync(new()
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
                }).GetAwaiter().GetResult();
                apiContext.PostAsync($"{Config.BaseUrl}/auth/app/token").GetAwaiter().GetResult();
                var types = apiContext.GetDefectTypesAsync().GetAwaiter().GetResult();
                return types.Select(t => new object[] { t }).ToList();
            }
            finally
            {
                apiContext?.DisposeAsync();
                playwright?.Dispose();
            }
        }

        [TestMethod]
        [Retry(2)]
        [AllureTmsItem("TMS-1003")]
        [DynamicData(nameof(GetDefectTypes))]
        public async Task CreateDefectsAsync(string defectType)
        {
            await AllureApi.Step("Перейти в фильтр \"Мои\"", async () =>
            {
                await Page.WaitForRequestFinishedAsync(new()
                {
                    Predicate = r => r.Url.Contains("api/defects/")
                });
                await Expect(DefectsPage.OfflineTitle).ToBeInViewportAsync(new() { Ratio = 1, Timeout = 5000 });
                await Expect(DefectsPage.OfflineTitle).ToBeVisibleAsync(new() { Timeout = 30000, Visible = false });

                await Expect(DefectsPage.NoButton).ToBeVisibleAsync();
                await Expect(DefectsPage.NoButton).ToBeEnabledAsync();
                await DefectsPage.NoButton.ClickAsync();
                await Expect(DefectsPage.MeFilter).ToBeVisibleAsync();
                await Expect(DefectsPage.MeFilter).ToBeEnabledAsync();
                await DefectsPage.MeFilter.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку \"Новый дефект\"", async () =>
            {
                await Expect(DefectsPage.NewDefectButton).ToBeVisibleAsync();
                await Expect(DefectsPage.NewDefectButton).ToBeEnabledAsync();
                await DefectsPage.NewDefectButton.ClickAsync();
            });

            await AllureApi.Step("Выбрать \"Вид дефекта\"", async () =>
            {
                await Expect(CreateWin.TypeOfDefectCombobox).ToBeVisibleAsync();
                await Expect(CreateWin.TypeOfDefectCombobox).ToBeEnabledAsync();
                await CreateWin.TypeOfDefectCombobox.ClickAsync();
                await Expect(CreateWin.ListOfDefectTypes.GetByText(defectType, new() { Exact = true }))
                    .ToBeVisibleAsync();
                await Expect(CreateWin.ListOfDefectTypes.GetByText(defectType, new() { Exact = true }))
                    .ToBeEnabledAsync();
                await CreateWin.ListOfDefectTypes.GetByText(defectType, new() { Exact = true }).ClickAsync();
            });

            await AllureApi.Step("Заполнить \"Описание дефекта\"", async () =>
            {
                await Expect(CreateWin.DescriptionDefectsTextBox).ToBeVisibleAsync();
                await Expect(CreateWin.DescriptionDefectsTextBox).ToBeEnabledAsync();
                await CreateWin.DescriptionDefectsTextBox.ClickAsync();
                await CreateWin.DescriptionDefectsTextBox.PressSequentiallyAsync(Config.TextPattern);
            });

            await AllureApi.Step("Нажать на кнопку выбора объекта из справочника", async () =>
            {
                await Expect(CreateWin.ObjectBookButton).ToBeVisibleAsync();
                await Expect(CreateWin.ObjectBookButton).ToBeEnabledAsync();
                await CreateWin.ObjectBookButton.ClickAsync();
                await ObjectWin.FullTreeSwitch.ClickAsync();
            });

            await AllureApi.Step("Раскрыть дерево и выбрать объект", async () =>
            {
                foreach (var item in ObjectTree)
                {
                    await Page.WaitForRequestFinishedAsync(new()
                    {
                        Predicate = r => r.Url.Contains("onlyUserAOR=false"),
                        Timeout = 300000
                    });
                    try
                    {
                        await Expect(Page.GetByText(item, new() { Exact = true })).ToBeVisibleAsync(new() { Timeout = 300000 });

                        if (item == ObjectTree.Last())
                        {
                            await Expect(Page.GetByText(item, new() { Exact = true }))
                                .ToBeVisibleAsync();
                            await Page.GetByText(item, new() { Exact = true }).ClickAsync();
                        }
                        else
                        {
                            await Page.ScrollWhileUnvisibleAsync(
                                Page.BuildCollapsedButton(item),
                                ObjectWin.ScrollAria);
                            await Page.BuildCollapsedButton(item).ClickAsync();
                        }
                    }
                    catch (Exception ex) when (ex is not AssertFailedException)
                    {
                        throw new InvalidOperationException(
                            $"Шаг «Раскрыть дерево и выбрать объект»: элемент «{item}»: {ex.Message}", ex);
                    }
                }
            });

            await AllureApi.Step("Нажать на кнопку \"Выбрать\"", async () =>
            {
                await Expect(ObjectWin.SelectButton).ToBeVisibleAsync();
                await Expect(ObjectWin.SelectButton).ToBeEnabledAsync();
                await ObjectWin.SelectButton.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку выбора оборудования из справочника", async () =>
            {
                await Expect(CreateWin.EquipmentBookButton).ToBeVisibleAsync();
                await Expect(CreateWin.EquipmentBookButton).ToBeEnabledAsync();
                await CreateWin.EquipmentBookButton.ClickAsync();
            });

            await AllureApi.Step("Выбрать чек-бокс оборудования", async () =>
            {
                await Expect(Page.GetByRole(AriaRole.Gridcell, new() { Name = EquipmentName })
                    .GetByRole(AriaRole.Checkbox).First).ToBeVisibleAsync();
                await Page.GetByRole(AriaRole.Gridcell, new() { Name = EquipmentName })
                    .GetByRole(AriaRole.Checkbox).First.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку \"Выбрать\"", async () =>
            {
                await Expect(EquipWin.SelectButton).ToBeVisibleAsync();
                await Expect(EquipWin.SelectButton).ToBeEnabledAsync();
                await EquipWin.SelectButton.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку выбора отвественного из справочника", async () =>
            {
                await Expect(CreateWin.ResponsibleBookButton).ToBeVisibleAsync();
                await Expect(CreateWin.ResponsibleBookButton).ToBeEnabledAsync();
                await CreateWin.ResponsibleBookButton.ClickAsync();
            });

            await AllureApi.Step("Раскрыть дерево и выбрать персонал", async () =>
            {
                foreach (var item in PersonTree)
                {
                    try
                    {
                        await Expect(Page.GetByRole(AriaRole.Gridcell, new() { Name = item })
                                .Filter(new() { Has = Page.Locator(".mal-ico") }))
                            .ToBeVisibleAsync(new() { Timeout = 300000 });

                        if (item == PersonTree.Last())
                        {
                            await Expect(Page.GetByRole(AriaRole.Gridcell, new() { Name = item })
                                    .Filter(new() { Has = Page.Locator(".mal-ico") }))
                                .ToBeVisibleAsync();
                            await Expect(Page.GetByRole(AriaRole.Gridcell, new() { Name = item })
                                    .Filter(new() { Has = Page.Locator(".mal-ico") }))
                                .ToBeEnabledAsync();
                            await Page.GetByRole(AriaRole.Gridcell, new() { Name = item })
                                .Filter(new() { Has = Page.Locator(".mal-ico") })
                                .ClickAsync();
                        }
                        else
                        {
                            await Expect(Page.BuildCollapsedButton(item)).ToBeVisibleAsync();
                            await Expect(Page.BuildCollapsedButton(item)).ToBeEnabledAsync();
                            await Page.BuildCollapsedButton(item).ClickAsync();
                        }
                    }
                    catch (Exception ex) when (ex is not AssertFailedException)
                    {
                        throw new InvalidOperationException(
                            $"Шаг «Раскрыть дерево и выбрать персонал»: элемент «{item}»: {ex.Message}", ex);
                    }
                }
            });

            await AllureApi.Step("Нажать на кнопку \"Выбрать\"", async () =>
            {
                await Expect(ResWin.SelectButton).ToBeVisibleAsync();
                await Expect(ResWin.SelectButton).ToBeEnabledAsync();
                await ResWin.SelectButton.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку \"Сохранить\"", async () =>
            {
                await Expect(CreateWin.SaveButton).ToBeVisibleAsync();
                await Expect(CreateWin.SaveButton).ToBeEnabledAsync();
                await CreateWin.SaveButton.ClickAsync();
            });

            await AllureApi.Step("Проверить, что статус записи поменялся на \"Новый\"", async () =>
            {
                await Expect(Page.GetByTitle("Новый")).ToBeVisibleAsync();
            });
        }

        [TestCleanup]
        [AllureAfter]
        public void CleanUp()
        {
            ObjectTree.Clear();
            PersonTree.Clear();
        }
    }
}