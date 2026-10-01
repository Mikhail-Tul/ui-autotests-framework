using Allure.Net.Commons;
using BaseProject.AllureHelpers;
using ShiftManagerTests.BaseShiftManager;
using System.Text.RegularExpressions;
using Allure.Net.Commons.Attributes;

namespace ShiftManagerTests.Tests
{
    [TestClass]
    [BaseAttributeSmoke]
    [AllureEpic("ShiftManager")]
    [AllureFeature("Принудительная передача смены")]
    [AllureSuiteHierarchy(
        "ShiftManager - (Дежурный персонал)",
        "Принудительная передача смены")]
    public sealed class ForceShiftTransfer : Auth
    {
        [TestInitialize]
        [AllureBefore]
        public async Task Init()
        {
            PersonTree = await APIContext.GetTreePerson(Config.PersonName);
            ShiftUids = await APIContext.GetShiftUidsAsync(Config.ShiftRole, Config.PersonName);

            var json = new JsonBuilder()
                .Add("role", ShiftUids[0])
                .Add("shift", ShiftUids[1])
                .AddObject("handover", b => b
                    .AddNull("uid")
                    .Add("password", ""))
                .AddObject("changer", b => b
                    .Add("uid", ShiftUids[2])
                    .Add("password", Config.Password))
                .Add("offset", 3)
                .AddArrayOfObjects("plugins", b => b
                    .AddArrayItem(a => a
                        .Add("name", "ReportPlugin")
                        .AddArray("data", "")))
                .Add("understudy", false)
                .Add("helper", false)
                .AddEmptyArray("handoverCrewMembers")
                .ToJson();

            var res = await APIContext.PostAsync($"{Config.BaseUrl}{Config.ShiftChangeApiPath}", new() { Data = json });
            if (!res.Ok)
                throw new InvalidOperationException(
                    $"API: HTTP {(int)res.Status} при смене (передача смены). Тело ответа: {await res.TextAsync()}");
        }

        [TestMethod]
        [Retry(2)]
        [AllureTmsItem("TMS-1006")]
        [AllureStory("Принудительная передача смены (Завершение работы)")]
        public async Task ForceShiftTransferAsync()
        {
            await AllureApi.Step("В настройках отображения выбрать чек-бокс \"Только с персоналом на смене\"", async () =>
            {
                await Expect(Page.GetByText(PersonTree[0])).ToBeInViewportAsync(new() { Ratio = 1, Timeout = 30000 });

                await Expect(ShiftMPage.DisplaySettingsDropDown).ToBeVisibleAsync();

                await ShiftMPage.DisplaySettingsDropDown.HoverAsync();

                await Expect(ShiftMPage.OnlyWithTheShiftStaffCheckBox).ToBeVisibleAsync();

                await ShiftMPage.OnlyWithTheShiftStaffCheckBox.ClickAsync();
            });

            await AllureApi.Step("Раскрыть дерево", async () =>
            {
                foreach (var item in PersonTree)
                {
                    try
                    {
                        await Expect(Page.GetByText(item)).ToBeVisibleAsync();

                        if (item == Config.PersonsDepartment)
                        {
                            await Page.GetByText(item).ClickAsync(new() { Timeout = 3000 });
                            return;
                        }
                        else
                        {
                            await Page.BuildCollapsedButton(item).ClickAsync(new() { Timeout = 3000 });
                        }
                    }
                    catch (Exception ex) when (ex is not AssertFailedException)
                    {
                        throw new InvalidOperationException(
                            $"Шаг «Раскрыть дерево»: элемент «{item}»: {ex.Message}", ex);
                    }
                }
            });

            await AllureApi.Step("Нажать на персонал", async () =>
            {
                await Expect(ShiftMPage.PersonGridcell).ToBeVisibleAsync();

                await ShiftMPage.PersonGridcell.ClickAsync();
            });

            await AllureApi.Step("Нажать на кнопку \"Принудительная передача смены\"", async () =>
            {
                await Expect(ShiftMPage.ForceShiftTransferButton).ToBeVisibleAsync();

                await ShiftMPage.ForceShiftTransferButton.ClickAsync();
            });

            await AllureApi.Step("Выбрать принимающим пользователем \"Завершение работы\"", async () =>
            {
                await Expect(ShiftTransferWin.HostPersonComboBox).ToBeVisibleAsync();
                await ShiftTransferWin.HostPersonComboBox.ClickAsync();
                await Expect(ShiftTransferWin.EndWorkItem).ToBeVisibleAsync();
                await ShiftTransferWin.EndWorkItem.ClickAsync();
                await Expect(ShiftTransferWin.HostPersonComboBox).ToHaveValueAsync("Завершение работы");
            });

            await AllureApi.Step("Заполнить причину", async () =>
            {
                await Expect(ShiftTransferWin.ReasonTextBox).ToBeVisibleAsync();

                await ShiftTransferWin.ReasonTextBox.ClickAsync();
                await ShiftTransferWin.ReasonTextBox.PressSequentiallyAsync(Config.TextPattern);
            });

            await AllureApi.Step("Нажать на кнопку \"Передать смену\"", async () =>
            {
                await Expect(ShiftTransferWin.ShiftTransferButton).ToBeVisibleAsync();

                await ShiftTransferWin.ShiftTransferButton.ClickAsync();
            });

            await AllureApi.Step("Проверить отображение уведомления об успешной передаче смены", async () =>
            {
                await Expect(Page.GetByText(new Regex(@$"Сдана\s+.*?\s+.*?\s+пользователем\s+{Config.PersonName}\s+в\s+роли\s+{Config.ShiftRole}")))
                    .ToBeInViewportAsync();
            });
        }
    }
}
