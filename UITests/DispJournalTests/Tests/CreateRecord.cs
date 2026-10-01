using BaseProject.Base;
using DispJournalTests.BaseDisp;
using DispJournalTests.Extensions;
using BaseProject.AllureHelpers;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons;

namespace DispJournalTests.Tests
{
    [TestClass]
    [BaseAttributeSmoke]
    [DataRowArgsAspect]
    [AllureBddHierarchy(
        "DispJournal",
        "Создание записи",
        "Создание записи")]
    [AllureSuiteHierarchy(
        "DispJournal - (Оперативный журнал)",
        "Создание записи")]
    public class CreateRecord : Auth
    {
        [TestInitialize]
        [AllureBefore]
        public async Task Init()
        {
            SubstationName = await APIContext.GetSubstationName();
            SubstationTree = await APIContext.GetSubstationTree(SubstationName);
            PersonTree = await APIContext.GetTreePerson(Config.PersonName);
        }

        public static IEnumerable<object[]> GetData()
        {
            var playwr = Playwright.CreateAsync().GetAwaiter().GetResult();
            IAPIRequestContext? api = null;
            try
            {
                api = playwr.APIRequest.NewContextAsync(new()
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

                var res = api.PostAsync($"{Config.BaseUrl}/auth/app/token")
                    .GetAwaiter().GetResult();
                if (!res.Ok)
                    throw new InvalidOperationException(
                        $"API: HTTP {(int)res.Status} при авторизации (auth/app/token). Тело ответа: {res.TextAsync().GetAwaiter().GetResult()}");

                CategoryTypes = api.GetCategoryTypeAsync().GetAwaiter().GetResult();

                foreach (var item in CategoryTypes)
                {
                    Categorys.Add(item, api.GetCategorysAsync(item).GetAwaiter().GetResult());
                }

                foreach (var item in Categorys)
                {
                    foreach (var item1 in item.Value)
                    {
                        yield return new object[] { item.Key, item1 };
                    }
                }
            }
            finally
            {
                api?.DisposeAsync().GetAwaiter().GetResult();
                playwr.Dispose();
            }
        }

        [TestMethod]
        [Retry(2)]
        [AllureTmsItem("TMS-1004")]
        //[DynamicData(nameof(GetData))]
        public async Task CreateRecordAsync(/*string categoryType, string category*/)
        {
            await AllureApi.Step("Нажать на кнопку \"Новая запись\"", async () =>
            {
                await Page.WaitForRequestFinishedAsync(new()
                {
                    Predicate = r => r.Url.Contains("api/workorders/records/getByStatus/count"),
                    Timeout = 300000
                });
                await Expect(DispPage.NewRecordButton).ToBeVisibleAsync();
                await Expect(DispPage.NewRecordButton).ToBeEnabledAsync();
                await DispPage.NewRecordButton.ClickAsync();
            });

            await AllureApi.Step($"Выбрать \"Управление бригадами\"", async () =>
            {
                await Expect(NewRecordWin.CategoryBookButton).ToBeVisibleAsync();
                await Expect(NewRecordWin.CategoryBookButton).ToBeEnabledAsync();
                await NewRecordWin.CategoryBookButton.ClickAsync();
                var count = await Page.GetByRole(AriaRole.Gridcell, new() { Name = "Управление бригадами" }).CountAsync();

                if (count < 2)
                {
                    await Page.ScrollWhileUnvisibleAsync(
                        Page.BuildCollapsedButton("Управление бригадами"),
                        Page.GetByTestId("dx-scroll-view-51d070")
                        .Filter(new() { Has = Page.GetByTestId("dx-tree-list-8f7fdc") }));
                    await Page.BuildCollapsedButton("Управление бригадами").ClickAsync();
                }
                else
                {
                    await Page.ScrollWhileUnvisibleAsync(
                        Page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new($@".+\s*Управление\sбригадами$") })
                                .Locator(".dx-treelist-collapsed"),
                        Page.GetByTestId("dx-scroll-view-51d070")
                        .Filter(new() { Has = Page.GetByTestId("dx-tree-list-8f7fdc") }));
                    await Page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new($@".+\s*Управление\sбригадами$") })
                                .Locator(".dx-treelist-collapsed")
                                .ClickAsync();
                }
            });

            await AllureApi.Step($"Выбрать \"Выезды бригад\"", async () =>
            {
                var count = await Page.GetByRole(AriaRole.Gridcell, new() { Name = "Выезды бригад" }).CountAsync();

                if (count < 2 && count > 0)
                {
                    await Page.ScrollWhileUnvisibleAsync(
                        Page.GetByText("Выезды бригад"),
                        Page.GetByTestId("dx-scroll-view-51d070")
                        .Filter(new() { Has = Page.GetByTestId("dx-tree-list-8f7fdc") }));
                    await Page.GetByText("Выезды бригад").ClickAsync();
                }
                else
                {
                    await Page.ScrollWhileUnvisibleAsync(
                        Page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new(@$"^Выезды\sбригад\s+.*") }),
                        Page.GetByTestId("dx-scroll-view-51d070")
                        .Filter(new() { Has = Page.GetByTestId("dx-tree-list-8f7fdc") }));
                    await Page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new(@$"^Выезды\sбригад\s+.*") })
                                .ClickAsync();
                }
            });

            await AllureApi.Step("Нажать на кнопку \"Выбрать\"", async () =>
            {
                await Expect(NewRecordWin.CommandMessageCombobox).ToBeVisibleAsync();
                await Expect(NewRecordWin.CommandMessageCombobox).ToBeEnabledAsync();
                await SelectionOfCategory.SelectButton.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку выбора объекта", async () =>
            {
                await Expect(NewRecordWin.ObjectBookButton).ToBeVisibleAsync();
                await Expect(NewRecordWin.ObjectBookButton).ToBeEnabledAsync();
                await NewRecordWin.ObjectBookButton.ClickAsync();
            });

            await AllureApi.Step("Переключиться на полное дерево", async () =>
            {
                await Expect(SelectionOfObject.Switch).ToBeVisibleAsync();
                await Expect(SelectionOfObject.Switch).ToBeEnabledAsync();
                await SelectionOfObject.Switch.ClickAsync();
            });

            await AllureApi.Step("Раскрыть дерево и выбрать чек-бокс объекта", async () =>
            {
                await Page.WaitForRequestFinishedAsync(new()
                {
                    Predicate = r => r.Url.Contains("tree/full?NodeUid")
                });

                foreach (var item in SubstationTree)
                {
                    try
                    {
                        if (item == SubstationName)
                        {
                            await Page.ScrollWhileUnvisibleAsync(
                                Page.GetByText(item, new() { Exact = true }),
                                Page.Locator(".dx-treelist-container").
                                    Locator(".dx-scrollable-container"),
                                1000);
                            await Page.GetByRole(AriaRole.Gridcell, new() { Name = item })
                                .GetByRole(AriaRole.Checkbox)
                                .ClickAsync();
                        }
                        else
                        {
                            await Page.ScrollWhileUnvisibleAsync(
                                Page.GetByText(item, new() { Exact = true }),
                                Page.Locator(".dx-treelist-container").
                                    Locator(".dx-scrollable-container"));
                            await Page.GetByText(item, new() { Exact = true }).ClickAsync();
                        }
                    }
                    catch (Exception ex) when (ex is not AssertFailedException)
                    {
                        throw new InvalidOperationException(
                            $"Шаг «Раскрыть дерево и выбрать чек-бокс объекта»: элемент «{item}»: {ex.Message}", ex);
                    }
                }
            });

            await AllureApi.Step("Нажать на кнопку \"Выбрать\"", async () =>
            {
                await Expect(SelectionOfObject.SelectButton).ToBeVisibleAsync();
                await Expect(SelectionOfObject.SelectButton).ToBeEnabledAsync();
                await SelectionOfObject.SelectButton.ClickAsync();
            });

            await AllureApi.Step("Выбрать \"Направление\"", async () =>
            {
                await Expect(NewRecordWin.CommandMessageCombobox).ToBeVisibleAsync();
                await Expect(NewRecordWin.CommandMessageCombobox).ToBeEnabledAsync();
                await NewRecordWin.CommandMessageCombobox.ClickAsync();
                await Expect(NewRecordWin.DirectionMessage).ToBeVisibleAsync();
                await Expect(NewRecordWin.DirectionMessage).ToBeEnabledAsync();
                await NewRecordWin.DirectionMessage.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку выбора персонала из справочника", async () =>
            {
                await Expect(NewRecordWin.CommandMessageBookButton).ToBeVisibleAsync();
                await Expect(NewRecordWin.CommandMessageBookButton).ToBeEnabledAsync();
                await NewRecordWin.CommandMessageBookButton.ClickAsync();
            });

            await AllureApi.Step("Раскрыть дерево", async () =>
            {
                foreach (var item in PersonTree)
                {
                    if (item == Config.PersonName) { return; }
                    try
                    {
                        await Page.ScrollWhileUnvisibleAsync(
                            Page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new(@$".*\s{item}$") }),
                            Page.GetByTestId("dx-tree-list-b78446").Locator(".dx-scrollable-container"));
                        await Page.GetByRole(AriaRole.Gridcell, new() { NameRegex = new(@$".*\s{item}$") })
                            .ClickAsync();
                    }
                    catch (Exception ex) when (ex is not AssertFailedException)
                    {
                        throw new InvalidOperationException(
                            $"Шаг «Раскрыть дерево персонала»: элемент «{item}»: {ex.Message}", ex);
                    }
                }
            });

            await AllureApi.Step("Выбрать чек-бокс персонала", async () =>
            {
                await Page.ScrollWhileUnvisibleAsync(
                        Page.GetByRole(AriaRole.Gridcell, new() { Name = Config.PersonName })
                            .GetByRole(AriaRole.Checkbox),
                        Page.GetByTestId("dx-tree-list-b78446").Locator(".dx-scrollable-container"));

                await Page.GetByRole(AriaRole.Gridcell, new() { Name = Config.PersonName })
                    .GetByRole(AriaRole.Checkbox).ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку \"Выбрать\"", async () =>
            {
                await Expect(SelectionOfPerson.SelectButton).ToBeVisibleAsync();
                await Expect(SelectionOfPerson.SelectButton).ToBeEnabledAsync();
                await SelectionOfPerson.SelectButton.ClickAsync();
            });

            await AllureApi.Step("Заполнить содержимое записи", async () =>
            {
                await Expect(NewRecordWin.EditorContenTextBox).ToBeVisibleAsync();
                await Expect(NewRecordWin.EditorContenTextBox).ToBeEditableAsync();
                await NewRecordWin.EditorContenTextBox.ClickAsync();
                await NewRecordWin.EditorContenTextBox.PressSequentiallyAsync(Config.TextPattern + " " + DispGuidRecord);
            });

            await AllureApi.Step("Нажать на кнопку \"Добавить\"", async () =>
            {
                await Expect(NewRecordWin.AddButton).ToBeVisibleAsync();
                await Expect(NewRecordWin.AddButton).ToBeEnabledAsync();
                await NewRecordWin.AddButton.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку \"Добавить запись\"", async () =>
            {
                await Expect(SaveRecordWin.AddRecordButton).ToBeVisibleAsync();
                await Expect(SaveRecordWin.AddRecordButton).ToBeEnabledAsync();
                await SaveRecordWin.AddRecordButton.ClickAsync();
                await Expect(Page.FilterForDispRecords(Config.TextPattern + " " + DispGuidRecord)).ToBeVisibleAsync();
            });
        }
    }
}
