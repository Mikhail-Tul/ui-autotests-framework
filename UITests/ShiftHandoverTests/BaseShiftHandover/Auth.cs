using Allure.Net.Commons.Attributes;
using BaseProject.Base;
using ShiftHandoverTests.BaseShiftManager;
using ShiftHandoverTests.Extensions;

namespace ShiftHandoverTests.BaseShiftHandover
{
    [TestClass]
    public class Auth : Base
    {
        [TestInitialize]
        [AllureStep("Открыть главную страницу приложения")]
        public async Task AuthAsync()
        {
            await AllureApi.Step("Подготовить данные и авторизация", async () =>
            {
                ShiftUids = await APIContext.GetShiftUidsAsync(Config.ShiftRole, Config.PersonName);
                await AuthorizeAsync("shifthandover");
            });
        }
    }
}
