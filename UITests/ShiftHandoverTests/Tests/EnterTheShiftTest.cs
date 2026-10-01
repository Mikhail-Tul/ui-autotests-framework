using Allure.Net.Commons.Attributes;
using ShiftHandoverTests.BaseShiftHandover;
using System.Text.RegularExpressions;

namespace ShiftHandoverTests.Tests
{
    [TestClass]
    [BaseAttributeSmoke]
    [AllureSuiteHierarchy(
        "ShiftHandover - (Прием-передача смены)",
        "Прием смены")]
    [AllureBddHierarchy(
        "ShiftHandover",
        "Прием смены",
        "Приме смены вне бригады")]
    public sealed class EnterTheShift : Auth
    {
        [TestInitialize]
        [AllureBefore]
        public async Task Init()
        {
            var json = new JsonBuilder()
                .Add("role", ShiftUids[0])
                .Add("shift", ShiftUids[1])
                .AddObject("changer", b => b
                    .AddNull("uid")
                    .Add("password", ""))
                .AddObject("handover", b => b
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
        public async Task EnterTheShiftAsync()
        {
            await AllureApi.Step("Выбрать роль для принятия смены", async () =>
            {
                await Expect(ShiftHandPage.OfflineTitle).ToBeInViewportAsync(new() { Ratio = 0, Timeout = 30000 });
                await Expect(ShiftHandPage.RoleTextBox).ToBeVisibleAsync();
                await ShiftHandPage.RoleTextBox.ClickAsync();
                await Expect(ShiftHandPage.Role).ToBeVisibleAsync();
                await ShiftHandPage.Role.ClickAsync();
                await Expect(ShiftHandPage.RoleTextBox).ToHaveValueAsync(Config.ShiftRole);
            });
            await AllureApi.Step("Ввести пароль принемающего смену", async () =>
            {
                await Expect(ShiftHandPage.BeginPasswordTextBox).ToBeVisibleAsync();
                await ShiftHandPage.BeginPasswordTextBox.ClickAsync();
                await ShiftHandPage.BeginPasswordTextBox.PressSequentiallyAsync(Config.Password);
                await Expect(ShiftHandPage.BeginPasswordTextBox).ToHaveValueAsync(Config.Password);
            });
            await AllureApi.Step("Нажать на кнопку \"Принять смену\"", async () =>
            {
                await Expect(ShiftHandPage.BeginOrEndWorkButton).ToBeVisibleAsync();
                await Expect(ShiftHandPage.BeginOrEndWorkButton).ToBeEnabledAsync();
                await ShiftHandPage.BeginOrEndWorkButton.ClickAsync();
            });
            await AllureApi.Step("Проверить отображение уведомления об успешном принятии смены", async () =>
            {
                await Expect(Page.GetByText(new Regex(@$"Принята\s+.*?\s+.*?\s+пользователем\s+{Config.PersonName}\s+в\s+роли\s+{Config.ShiftRole}"))).
                ToBeInViewportAsync();
            });
        }
    }
}
