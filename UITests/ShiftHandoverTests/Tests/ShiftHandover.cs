using Allure.Net.Commons.Attributes;
using ShiftHandoverTests.BaseShiftHandover;
using System.Text.RegularExpressions;

namespace ShiftHandoverTests.Tests
{
    [TestClass]
    [BaseAttributeSmoke]
    [AllureSuiteHierarchy(
        "ShiftHandover - (Прием-передача смены)",
        "Сдача смены")]
    [AllureBddHierarchy(
        "ShiftHandover",
        "Сдача смены",
        "Сдача смены вне бригады")]
    public sealed class ShiftHandover : Auth
    {
        [TestInitialize]
        [AllureBefore]
        public async Task Init()
        {
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
        [AllureTmsItem("TMS-1005")]
        public async Task ShiftHandoverAsync()
        {
            await AllureApi.Step("Проверить отображения принятой роли", async () =>
            {
                await Expect(ShiftHandPage.OfflineTitle).ToBeInViewportAsync(new() { Ratio = 0, Timeout = 30000 });
                await Expect(ShiftHandPage.RoleTextBox).ToBeVisibleAsync();
                await Expect(ShiftHandPage.RoleTextBox).ToHaveValueAsync(Config.ShiftRole, new() { Timeout = 10000 });
            });
            await AllureApi.Step("Ввести пароль сдающего смену", async () =>
            {
                await Expect(ShiftHandPage.EndPasswordTextBox).ToBeVisibleAsync();
                await ShiftHandPage.EndPasswordTextBox.ClickAsync();
                await ShiftHandPage.EndPasswordTextBox.PressSequentiallyAsync(Config.Password);
                await Expect(ShiftHandPage.EndPasswordTextBox).ToHaveValueAsync(Config.Password);
            });
            await AllureApi.Step("Ввести рапорт сдающего", async () =>
            {
                await Expect(ShiftHandPage.RaportTextBox).ToBeVisibleAsync();
                await ShiftHandPage.RaportTextBox.ClickAsync();
                await ShiftHandPage.RaportTextBox.PressSequentiallyAsync(Config.TextPattern);
                await Expect(ShiftHandPage.RaportTextBox).ToHaveTextAsync(Config.TextPattern);
            });
            await AllureApi.Step("Нажать на кнопку \"Завершить работу\"", async () =>
            {
                await Expect(ShiftHandPage.BeginOrEndWorkButton).ToBeVisibleAsync();
                await Expect(ShiftHandPage.BeginOrEndWorkButton).ToBeEnabledAsync();
                await ShiftHandPage.BeginOrEndWorkButton.ClickAsync();
            });
            await AllureApi.Step("Проверить отображение уведомления об успошной сдаче смены", async () =>
            {
                await Expect(Page.GetByText(new Regex(@$"Сдана\s+.*?\s+.*?\s+пользователем\s+{Config.PersonName}\s+в\s+роли\s+{Config.ShiftRole}"))).
                    ToBeInViewportAsync();
            });
        }
    }
}
