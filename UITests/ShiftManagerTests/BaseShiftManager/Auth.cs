using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using BaseProject.Base;

namespace ShiftManagerTests.BaseShiftManager
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
                await AuthorizeAsync("shiftmanager");
            });
        }
    }
}
